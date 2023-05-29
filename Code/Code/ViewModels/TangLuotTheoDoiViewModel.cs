using Code.Models;
using Code.Utils.Story;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Code.ViewModels
{
    public class TangLuotTheoDoiViewModel:ViewModelBase
    {
        private static TangLuotTheoDoiViewModel INSTANCE = null;

        public static TangLuotTheoDoiViewModel GetInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new TangLuotTheoDoiViewModel();
            }
            return INSTANCE;
        }

        private string _duongDan;
        private int _soLuotTang;
        private string _trangThai;

        public string DuongDan
        {
            get { return _duongDan; }
            set
            {
                _duongDan = value;
                OnPropertyChanged("DuongDan");
            }
        }

        public int SoLuotTang
        {
            get { return _soLuotTang; }
            set
            {
                _soLuotTang = value;
                OnPropertyChanged("SoLuotTang");
            }
        }

        public string TrangThai
        {
            get { return _trangThai; }
            set
            {
                _trangThai = value;
                OnPropertyChanged("TrangThai");
            }
        }

        public TangLuotTheoDoiViewModel() : base()
        {

        }

        protected override void QuanLyCongViecChoCacThietBi(List<string> thietbi, long soLanLap, int ind)
        {
            soLanLap = SoLuotTang;
            var tbs = thietbi.ToHashSet();
            foreach (var tb in thietbi)
            {
                facebookOfDevice[tb] = new List<string>();
            }
            foreach (var ac in DataProvider.Ins.db.TaiKhoanFacebooks)
            {
                if (tbs.Contains(ac.IDThietBi.Trim()))
                {
                    facebookOfDevice[ac.IDThietBi.Trim()].Add(ac.TenDangNhap);
                }
            }
            base.QuanLyCongViecChoCacThietBi(thietbi, soLanLap, ind);
        }

        private Dictionary<String, List<String>> facebookOfDevice = new Dictionary<string, List<string>>();

        private bool KiemTraURLFacebook(string url)
        {
            Regex regex = new Regex("(?:(?:http|https):\\/\\/)?(?:www.)?facebook.com\\/(?:(?:\\w)*#!\\/)?(?:pages\\/)?(?:[?\\w\\-]*\\/)?(?:profile.php\\?id=(?=\\d.*))?([\\w\\-]*)?");

            if (DuongDan != null)
            {
                if (regex.IsMatch(DuongDan))
                {
                    return true;
                }
            }
            return false;
        }

        protected override void ExecuteShowPopUpWindow(object obj)
        {
            var canExcute = KiemTraURLFacebook(DuongDan);
            if (canExcute)
            {
                base.ExecuteShowPopUpWindow(obj);
            }
            else
            {
                MessageBox.Show("Thông tin nhập sai vui lòng nhập lại");
            }
        }

        protected override BaseScript createScriptToRun(string thietbiId, string url)
        {
            var tenDangNhap = facebookOfDevice[thietbiId].LastOrDefault();
            if (tenDangNhap == null)
            {
                return null;
            }
            facebookOfDevice[thietbiId].RemoveAt(facebookOfDevice[thietbiId].Count() - 1);
            return new DangKyKenhYoutubeScript(thietbiId, url, tenDangNhap);
        }
    }
}
