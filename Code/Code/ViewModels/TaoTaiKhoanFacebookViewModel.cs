using Code.Models;
using Code.Utils.Story;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.ViewModels
{
    public class TaoTaiKhoanFacebookViewModel:ViewModelBase
    {
        private string[] arrHo = { "Nguyen", "Tran", "Ngo", "Ha", "Đinh", "Phan" };
        private string[] arrTen = { "Duy", "Khanh", "Mai", "Huyen", "Quynh", "Linh", "Huong", "Hoang", "Nguyen", "Nam", "Anh" };

        private static TaoTaiKhoanFacebookViewModel INSTANCE = null;

        public static TaoTaiKhoanFacebookViewModel GetInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new TaoTaiKhoanFacebookViewModel();
            }
            return INSTANCE;
        }
        public TaoTaiKhoanFacebookViewModel() : base()
        {
          
        }
        protected override BaseScript createScriptToRun(string thietbiId, string url)
        {
            var account = AccountFactory.RandomFaceBookAccount(thietbiId);
            if (account == null)
            {
                return null;
            }
            /*var prefix = RandomString(10);
            Random rnd = new Random();
            var account = new TaiKhoanFacebook();
            account.Ten = arrTen[rnd.Next(0, arrTen.Length - 1)];
            account.TenDangNhap = "nguyenducmanh100619dta@gmail.com";
            account.Ho = arrHo[rnd.Next(0, arrHo.Length - 1)];
            account.MatKhau = "Abc13579@!";
            account.NamSinh = 2000 + rnd.Next(-10, 10);
            account.NgaySinh = rnd.Next(1, 28);
            account.ThangSinh = rnd.Next(0, 11);
            account.GioiTinh = rnd.Next(0, 2);
            account.IDThietBi = thietbiId;
            account.TrangThai = AccountStatus.PENDING;*/

            account = DataProvider.Ins.db.TaiKhoanFacebooks.Add(account);
            DataProvider.Ins.db.SaveChanges();

            var script = new TaoTaiKhoanFacebook(thietbiId, account);

            return script;
        }

    }
}
