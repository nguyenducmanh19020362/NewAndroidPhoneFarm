using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.ViewModels
{
    public class QuanLyHeThongViewModel:ViewModelBase
    {
        private int _SoThuTu { get; set; }
        private string _MaThietBi { get; set; }
        private int _SoTaiKhoanFacebookTC { get; set; }
        private int _SoTaiKhoanFacebookTB { get; set; }
        private int _SoTaiKhoanYoutubeTC { get; set; }
        private int _SoTaiKhoanYoutubeTB { get; set; }
        public string MaThietBi
        {
            get { return _MaThietBi; }
            set { _MaThietBi = value; OnPropertyChanged("MaThietBi"); }
        }
        public int SoThuTu
        {
            get { return _SoThuTu; }
            set { _SoThuTu = value; OnPropertyChanged("SoThuTu"); }
        }

        public int SoTaiKhoanFacebookTC
        {
            get { return _SoTaiKhoanFacebookTC; }
            set { _SoTaiKhoanFacebookTC = value; }
        }
        public int SoTaiKhoanFacebookTB
        {
            get { return _SoTaiKhoanFacebookTB; }
            set { _SoTaiKhoanFacebookTB = value; }
        }
        public int SoTaiKhoanYoutubeTC
        {
            get { return _SoTaiKhoanYoutubeTC; }
            set { _SoTaiKhoanYoutubeTC = value; }
        }
        public int SoTaiKhoanYoutubeTB
        {
            get { return _SoTaiKhoanYoutubeTB; }
            set { _SoTaiKhoanYoutubeTB = value; }
        }
        public QuanLyHeThongViewModel() { 

        }
    }
}
