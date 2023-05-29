﻿using AngleSharp.Dom;
using Code.Models;
using Code.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace Code.Utils.Story
{
    class TaoTaiKhoanFacebook: BaseScript
    {
        private readonly string facebook = "com.facebook.katana";
        private readonly string login = "com.facebook.katana/.LoginActivity";

        private readonly TaiKhoanFacebook account;
        private readonly ADBUtils adb;

        public TaoTaiKhoanFacebook(string deviceId, TaiKhoanFacebook account) : base()
        {
            this.account = account;
            this.account.IDThietBi = deviceId;
            this.adb = new ADBUtils(deviceId);
        }

        protected override void OnCompleted()
        {
            account.TrangThai = AccountStatus.CREATED;
            DataProvider.Ins.db.TaiKhoanFacebooks.AddOrUpdate(this.account);
            DataProvider.Ins.db.SaveChanges();
        }

        protected override void Init()
        {
            account.TrangThai = AccountStatus.CREATING;
            DataProvider.Ins.db.TaiKhoanFacebooks.AddOrUpdate(this.account);
            DataProvider.Ins.db.SaveChanges();
        }

        protected override void OnFailed()
        {
            account.TrangThai = AccountStatus.FAILED;
            DataProvider.Ins.db.TaiKhoanFacebooks.AddOrUpdate(this.account);
            DataProvider.Ins.db.SaveChanges();
        }
        
        protected override bool IsCompleted()
        {
            NodeHolder nodeHolder = new NodeHolder();
            var scriptGetCodeFacebook = new TakeFacebookCode(account.IDThietBi, nodeHolder);
            var script = new BaseScriptComponent();
            var stopAcivity = new BaseScriptComponent("Dừng facebook")
            {
                action = () =>
                {
                    adb.stopPackage(facebook);
                },
                onCompleted = () =>
                {
                    Thread.Sleep(500);
                }
            };
            var startFacebook = new BaseScriptComponent("Mở facebook")
            {
                action = () =>
                {
                    this.adb.startPackage(login);
                },
                onCompleted = () =>
                {
                    Thread.Sleep(500);
                }
            };
            var logout = checkLogout();
            var clickCreateNewAccount = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Create new account";
            }, 8, "Tạo mới tài khoản Facebook");


            var clickGetStart = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Get started";
            }, 8);

            var inputFirstName = inputText((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "First name";
            }, 8, account.Ho, "Nhập họ");

            var inputLastName = inputText((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Last name";
            }, 8, account.Ten, "Nhập tên");

            var clickNoneOfTheAbove = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "NONE OF THE ABOVE";
            }, 8);

            var clickNext1 = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Next";
            }, 8);

            var clickNext2 = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Next";
            }, 8);
            var clickNext3 = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Next";
            }, 8);
            var clickNext4 = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Next";
            }, 8);
            var clickNext5 = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Next";
            }, 8);

            var clickBirthdayInput = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Birthday";
            }, 8); ;

            var setBirthdayInput = setBirthDay((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "2001";
            },
            (XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "2023";
            },
            (XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "SET";
            }
            );

            var clickGender = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Female";
            }, 8, "Chọn giới tính");

            var clickSignUpWithEmail = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Sign up with email";
            }, 8, "Chọn đăng nhập bằng email");

            var clickAllow = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "DENY";
            }, 8);

            var inputEmail = inputInfo(account.TenDangNhap, "Nhập tên tài khoản");
            var inputPass = inputInfo(account.MatKhau, "Nhập mật khẩu");
            var clickNotNow = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "Not now";
            }, 8);
            var clickIAgree = waitAndClick((XmlNode n) =>
            {
                return n.Attributes["text"].InnerText == "I agree";
            }, 8, "Chấp nhận điều khoản Facebook");
            

            script.AddNext(stopAcivity
                .AddNext(startFacebook
                .AddNext(logout
                .AddNext(clickCreateNewAccount
                .AddNext(clickGetStart
                //chỗ này comment lại
                .AddNext(clickNoneOfTheAbove
                .AddNext(inputFirstName
                .AddNext(inputLastName
                .AddNext(clickNext1
                .AddNext(clickBirthdayInput
                .AddNext(setBirthdayInput
                .AddNext(clickNext2
                .AddNext(clickGender
                .AddNext(clickNext3
                .AddNext(clickAllow
                .AddNext(clickSignUpWithEmail
                .AddNext(inputEmail
                .AddNext(clickNext4
                .AddNext(inputPass
                .AddNext(clickNext5
                .AddNext(clickNotNow
                .AddNext(clickIAgree))))))))))))))))))))
                //comment ngoặc kép
                ));
            var isTrue = false;
            script.onTitleChange = onTitleChange;
            if (script.RunScript())
            {
                Thread.Sleep(5000);
                scriptGetCodeFacebook.onTitleChange = onTitleChange;
                if (scriptGetCodeFacebook.RunScript())
                {
                    var codeFacebook = TakeFacebookCode.GetCode(nodeHolder.node);
                    Console.WriteLine(codeFacebook);
                    var inputCode = inputInfo(codeFacebook);
                    var newScript = new BaseScriptComponent();
                    newScript.AddNext(
                        startFacebook.AddNext(inputCode)
                        );
                    newScript.onTitleChange = onTitleChange;
                    isTrue = newScript.RunScript();
                }
            }
            return isTrue;
        }

        private BaseScriptComponent checkLogout()
        {
            XmlNode node = null;
            return new BaseScriptComponent(-1)
            {
                action = () =>
                {
                    Thread.Sleep(5000);
                    var screen = this.adb.getCurrentView();
                    var needView = ViewUtils.findNode(screen, new Matcher((XmlNode n) =>
                        {
                            return n.Attributes["text"].InnerText == "Enter the confirmation code";
                        }
                    ));
                    node = needView.FirstOrDefault();
                    if (node != null)
                    {
                        adb.tap(40, 90);
                        Thread.Sleep(3000);
                        screen = this.adb.getCurrentView();
                        needView = ViewUtils.findNode(screen, new Matcher((XmlNode n) =>
                        {
                            return n.Attributes["text"].InnerText == "LEAVE";
                        }
                        ));
                        node = needView.FirstOrDefault();
                        if (node != null)
                        {
                            Console.WriteLine("ok");
                            var b = Bound.ofXMLNode(node);
                            var x = b.x + b.h / 2;
                            var y = b.y + b.w / 2;
                            adb.tap(x, y);
                        }
                    }
                    else
                    {
                        needView = ViewUtils.findNode(screen, new Matcher((XmlNode n) =>
                        {
                            return n.Attributes["text"].InnerText == "Create new account";
                        }));
                        node = needView.FirstOrDefault();
                        if (node == null)
                        {
                            screen = this.adb.getCurrentView();
                            needView = ViewUtils.findNode(screen, new Matcher((XmlNode n) =>
                            {
                                return n.Attributes["selected"].InnerText == "true";
                            }));
                            node = needView.FirstOrDefault();
                            if (node != null)
                            {
                                var b = Bound.ofXMLNode(node);
                                /*var x = 1000;
                                var y = b.y + b.w / 2;*/
                                var x = 710;
                                var y = 160;
                                adb.tap(x, y);
                            }
                            Thread.Sleep(1000);
                            adb.swipe(100, 1000, 100, 100);
                            Thread.Sleep(1000);
                            screen = this.adb.getCurrentView();
                            needView = ViewUtils.findNode(screen, new Matcher((XmlNode n) =>
                            {
                                return n.Attributes["content-desc"].InnerText == "Log out";
                            }));
                            node = needView.FirstOrDefault();
                            if (node != null)
                            {
                                var b = Bound.ofXMLNode(node);
                                var x = b.x + b.h / 2;
                                var y = b.y + b.w / 2;
                                adb.tap(x, y);
                            }

                            screen = this.adb.getCurrentView();
                            needView = ViewUtils.findNode(screen, new Matcher((XmlNode n) =>
                            {
                                return n.Attributes["text"].InnerText == "LOG OUT";
                            }));
                            node = needView.FirstOrDefault();
                            if (node != null)
                            {
                                var b = Bound.ofXMLNode(node);
                                var x = b.x + b.h / 2;
                                var y = b.y + b.w / 2;
                                adb.tap(x, y);
                            }
                        }
                    }
                }
            };
        }

        private BaseScriptComponent inputInfo(string text, string title = null)
        {
            return new BaseScriptComponent(title, -1)
            {

                action = () =>
                {
                    adb.typeText(text);
                    Thread.Sleep(1000);
                }
            };
        }

        private BaseScriptComponent inputText(Matcher matcher, double maxWait, string text, string title = null, int clickNumber = 1)
        {
            XmlNode node = null;
            return new BaseScriptComponent(title, -1)
            {
                init = () =>
                {
                    Thread.Sleep(1000);
                },
                canAction = () =>
                {
                    var screen = this.adb.getCurrentView();
                    var needView = ViewUtils.findNode(screen, matcher);
                    node = needView.FirstOrDefault();
                    return needView.Count != 0;
                },
                action = () =>
                {
                    var b = Bound.ofXMLNode(node);
                    var x = b.x + b.h / 2;
                    var y = b.y + b.w / 2;
                    while (clickNumber-- != 0)
                    {
                        adb.tap(x, y);
                        Thread.Sleep(1000);
                        adb.typeText(text);
                        Thread.Sleep(1000);
                    }
                }
            };
        }

        private BaseScriptComponent waitAndClick(Matcher matcher, double maxWait, string title = null, int clickNumber = 1)
        {
            DateTime startTime = DateTime.UtcNow;
            XmlNode node = null;
            return new BaseScriptComponent(title, -1)
            {
                init = () =>
                {
                    Thread.Sleep(1000);
                    startTime = DateTime.UtcNow;
                },
                canAction = () =>
                {
                    var screen = this.adb.getCurrentView();
                    var needView = ViewUtils.findNode(screen, matcher);
                    node = needView.FirstOrDefault();
                    return needView.Count != 0;
                },
                wait = () =>
                {
                    Thread.Sleep(200);
                },
                action = () =>
                {
                    var b = Bound.ofXMLNode(node);
                    var x = b.x + b.h / 2;
                    var y = b.y + b.w / 2;
                    Console.WriteLine(x + " " + y);
                    while (clickNumber-- != 0)
                    {
                        adb.tap(x, y);
                        Thread.Sleep(200);
                    }
                },
                isError = () =>
                {
                    var t = System.DateTime.UtcNow - startTime;
                    return t.TotalSeconds > maxWait;
                }
            };
        }
        private BaseScriptComponent setBirthDay(Matcher matcher, Matcher matcherNumber, Matcher matcherSet, int maxTry = 12)
        {
            XmlNode node = null;
            XmlNode number = null;
            XmlNode setButton = null;
            var xSetButton = 0;
            var ySetButton = 0;
            var xNumber = 0;
            var yNumber = 0;
            return new BaseScriptComponent("Thiết lập ngày sinh", maxTry)
            {
                init = () =>
                {
                    Thread.Sleep(500);
                    var screen = this.adb.getCurrentView();
                    var numberView = ViewUtils.findNode(screen, matcherNumber);
                    number = numberView.FirstOrDefault();
                    screen = this.adb.getCurrentView();
                    var setView = ViewUtils.findNode(screen, matcherSet);
                    setButton = setView.FirstOrDefault();

                    var b1 = Bound.ofXMLNode(number);
                    xNumber = b1.x + b1.h / 2;
                    yNumber = b1.y + b1.w / 2;
                    var b2 = Bound.ofXMLNode(setButton);
                    xSetButton = b2.x + b2.h / 2;
                    ySetButton = b2.y + b2.w / 2;
                },
                canAction = () =>
                {
                    var screen = this.adb.getCurrentView();
                    var needView = ViewUtils.findNode(screen, matcher);
                    node = needView.FirstOrDefault();
                    return needView.Count != 0;
                },
                wait = () =>
                {
                    this.adb.swipe(xNumber, yNumber, xSetButton, ySetButton);
                    maxTry--;
                },
                action = () =>
                {
                    adb.tap(xSetButton, ySetButton);
                },
                isError = () =>
                {
                    return maxTry <= 0;
                }
            };
        }
    }
}
