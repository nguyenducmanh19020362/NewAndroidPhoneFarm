using AngleSharp.Dom;
using Code.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Code.Utils.Story
{
    public class XemVideoYoutubeScript : BaseScript
    {
        private readonly ADBUtils adb;
        private readonly string intent = "android.intent.action.VIEW";
        private readonly string youtube = "com.google.android.youtube";
        private readonly string url;
        private readonly int thoiGianXem;
        private bool isDone = false;

        public XemVideoYoutubeScript(string deviceId, string url, int thoiGianXem) : base()
        {
            this.url = url;
            this.thoiGianXem = thoiGianXem;
            this.adb = new ADBUtils(deviceId);
        }

        protected override void Action()
        {
            var script = new BaseScriptComponent();
            script.onTitleChange = onTitleChange;
            var stopAcivity = new BaseScriptComponent("Dừng Youtube")
            {
                action = () =>
                {
                    adb.stopPackage(youtube);
                },
                onCompleted = () =>
                {
                    Thread.Sleep(1000);
                }
            };
            var startYoutube = new BaseScriptComponent("Mở Youtube")
            {
                action = () =>
                {
                    adb.startIntent(intent, url);
                    Thread.Sleep(500);
                },
                onCompleted = () =>
                {
                    Thread.Sleep(10 * 1000);
                }
            };
            string view = "";          

            int watchedTime = 0;
            XmlNode adsCountdown = null;
            XmlNode adsSkip = null;

            var onAdsCountdown = new BaseScriptComponent("Đợi quảng cáo kết thúc")
            {
                action = () =>
                {
                    var time = adsCountdown.Attributes["content-desc"].InnerText.Split(' ').FirstOrDefault();
                    if (int.TryParse(time, out int delay))
                    {
                        Thread.Sleep(delay * 1000);
                    }
                },
                onCompleted = () =>
                {
                    view = adb.getCurrentView();
                }
            };

            var skipAds = new BaseScriptComponent("Bỏ qua quảng cáo")
            {
                action = () =>
                {
                    var b = Bound.ofXMLNode(adsSkip);
                    var x = b.x + b.h / 2;
                    var y = b.y + b.w / 2;
                    adb.tap(x, y);
                },
                onCompleted = () =>
                {
                    view = adb.getCurrentView();
                },
            };

            var rand = new Random();

            var watchVideo = new BaseScriptComponent("Xem video")
            {
                action = () =>
                {
                    var seconds = rand.Next(3) + 4;
                    seconds = Math.Min(seconds, this.thoiGianXem - watchedTime + 1);
                    Thread.Sleep(seconds * 1000);
                    watchedTime += seconds;
                },
                onCompleted = () =>
                {
                    view = adb.getCurrentView();
                }
            };

            var done = new BaseScriptComponent();

            Candidate hasAdsCountdown = () =>
            {
                adsCountdown = ViewUtils.findNode(view, (node) =>
                {
                    return "com.google.android.youtube:id/countdown_text" == node.Attributes["resource-id"].InnerText;
                }).FirstOrDefault();
                return adsCountdown != null;
            };

            Candidate hasAdsSkipButton = () =>
            {
                adsSkip = ViewUtils.findNode(view, (node) =>
                {
                    return "com.google.android.youtube:id/skip_ad_button" == node.Attributes["resource-id"].InnerText;
                }).FirstOrDefault();
                return adsSkip != null;
            };

            Candidate hasPlayVideo = () =>
            {
                var v = ViewUtils.findNode(view, (node) =>
                {
                    return !node.HasChildNodes &&
                        "com.google.android.youtube:id/watch_while_time_bar_view_overlay" == node.Attributes["resource-id"].InnerText;
                }).FirstOrDefault();
                return v != null;
            };

            int maxWaitTime = 20;

            var waitForYoutubeStart = new BaseScriptComponent("Đợi Youtube khởi động")
            {
                init = () =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine(maxWaitTime);
                },
                action = () =>
                {
                    --maxWaitTime;
                    view = adb.getCurrentView();
                },
                isError = () =>
                {
                    return maxWaitTime <= 0;
                }
            };

            waitForYoutubeStart.AddNext(watchVideo, hasPlayVideo);
            waitForYoutubeStart.AddNext(onAdsCountdown, hasAdsCountdown);
            waitForYoutubeStart.AddNext(skipAds, hasAdsSkipButton);
            waitForYoutubeStart.AddNext(waitForYoutubeStart);

            var watchAction = new BaseScriptComponent();
            watchAction.AddNext(done, () =>
            {
                return watchedTime >= this.thoiGianXem;
            });
            watchAction.AddNext(onAdsCountdown, hasAdsCountdown);
            watchAction.AddNext(skipAds, hasAdsSkipButton);
            watchAction.AddNext(watchVideo);

            onAdsCountdown.AddNext(watchAction);

            skipAds.AddNext(watchAction);

            watchVideo.AddNext(watchAction);

            script.AddNext(stopAcivity.AddNext(startYoutube.AddNext(waitForYoutubeStart)));

            isDone = script.RunScript();
        }

        protected override bool IsCompleted()
        {
            return isDone;
        }
        private bool Old()
        {
            var script = new BaseScriptComponent();
            var stopAcivity = new BaseScriptComponent("Dừng Youtube")
            {
                action = () =>
                {
                    adb.stopPackage(youtube);
                },
                onCompleted = () =>
                {
                    Thread.Sleep(500);
                }
            };
            var startYoutube = new BaseScriptComponent("Mở Youtube")
            {
                action = () =>
                {
                    adb.startIntent(intent, url);
                    Thread.Sleep(500);
                },
                onCompleted = () =>
                {
                    Thread.Sleep(500);
                }
            };
            var skipAdv = new BaseScriptComponent("Kiểm tra quảng cáo và skip")
            {
                init = () =>
                {
                    Thread.Sleep(5000);
                },
                action = () =>
                {
                    var screen = this.adb.getCurrentView();
                    var needView = ViewUtils.findNode(screen, new Matcher((XmlNode n) =>
                    {
                        return n.Attributes["text"].InnerText == "Skip ads";
                    }));
                    if (needView.Count > 0)
                    {
                        var time = ViewUtils.findNode(screen, new Matcher((XmlNode n) =>
                        {
                            return n.Attributes["text"].InnerText.IndexOf("Ad") != -1;
                        }));
                        if (time.Count > 0)
                        {
                            var tmp = time.FirstOrDefault();
                            string txt = tmp.Attributes["text"].InnerText;
                            int vt = txt.IndexOf(":");
                            string txtPhut = txt.Substring(vt - 2, 2);
                            string txtGiay = txt.Substring(vt + 1, 2);
                            int minutes = int.Parse(txtPhut);
                            int seconds = int.Parse(txtGiay);
                            if (minutes > 0 || seconds > 10)
                            {
                                var node = needView.FirstOrDefault();
                                var b = Bound.ofXMLNode(node);
                                var x = b.x + b.h / 2;
                                var y = b.y + b.w / 2;
                                adb.tap(x, y);
                            }
                        }
                        
                    }
                }
            };
            var wait = new BaseScriptComponent("Xem video")
            {
                init = () =>
                {
                    Thread.Sleep((thoiGianXem + 20) * 1000);
                }
            };
            script.AddNext(
                stopAcivity.AddNext(
                    startYoutube.AddNext(
                        skipAdv.AddNext(
                            skipAdv.AddNext(
                                wait
                                )
                            )
                        )
                    )
                );

            script.onTitleChange = this.onTitleChange;
            return script.RunScript();
        }
    }
}
