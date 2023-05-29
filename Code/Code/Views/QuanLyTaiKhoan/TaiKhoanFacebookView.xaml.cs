using Code.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Code.Views.QuanLyTaiKhoan
{
    /// <summary>
    /// Interaction logic for TaiKhoanFacebookView.xaml
    /// </summary>
    public partial class TaiKhoanFacebookView : UserControl
    {
        public ObservableCollection<ThongTinTaiKhoan> taiKhoans;
        private BackgroundWorker bk = new BackgroundWorker();
        DbSet<TaiKhoanFacebook> objectList = null;
        public TaiKhoanFacebookView()
        {
            taiKhoans = new ObservableCollection<ThongTinTaiKhoan>();
            InitializeComponent();
            dgTaiKhoan.ItemsSource = taiKhoans;
            bk.DoWork += (obj, e) =>
            {
                objectList = DataProvider.Ins.db.TaiKhoanFacebooks;
            };
            bk.RunWorkerCompleted += (obj, e) =>
            {
                Load();
            };
            bk.RunWorkerAsync();

            removeItem.DoWork += (obj, e) =>
            {
                var items = (List<TaiKhoanGoogle>)e.Argument;
                DataProvider.Ins.db.TaiKhoanGoogles.RemoveRange(items);
                DataProvider.Ins.db.SaveChanges();
            };
            removeItem.RunWorkerCompleted += (obj, e) =>
            {
                MessageBox.Show("Xóa thành công");
            };
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            bk.RunWorkerAsync();
        }

        private void btnDeleteError_Click(object sender, RoutedEventArgs e)
        {
            var selectSet = new HashSet<int>();
            var items = new List<TaiKhoanFacebook>();
            foreach (var item in objectList)
            {
                if (AccountStatus.IsError(item.TrangThai))
                {
                    items.Add(item);
                    selectSet.Add(item.IDTaiKhoanF);
                }
            }
            var rItems = new List<ThongTinTaiKhoan>();
            foreach (var tk in taiKhoans)
            {
                if (selectSet.Contains(tk.ID))
                {
                    rItems.Add(tk);
                }
            }
            foreach (var item in rItems)
            {
                taiKhoans.Remove(item);
            }
            removeItem.RunWorkerAsync(items);
        }
        private void Load()
        {
            taiKhoans.Clear();
            int stt = 1;
            foreach (var item in objectList)
            {
                ThongTinTaiKhoan taiKhoan = new ThongTinTaiKhoan();
                taiKhoan.ID = item.IDTaiKhoanF;
                taiKhoan.STT = stt++;
                taiKhoan.TenDangNhap = item.TenDangNhap;
                taiKhoan.MatKhau = item.MatKhau;
                taiKhoan.HoTen = string.Format("{0} {1}", item.Ho, item.Ten);
                taiKhoan.GioiTinh = item.GioiTinh == 0 ? "Nữ" : item.GioiTinh == 1 ? "Nam" : "Không";
                taiKhoan.NgaySinh = string.Format("{0}/{1}/{2}", item.NgaySinh, item.ThangSinh, item.NamSinh);
                taiKhoan.TrangThai = AccountStatus.GetDescription(item.TrangThai);
                taiKhoan.MaThietBi = item.IDThietBi;
                taiKhoans.Add(taiKhoan);
            }
            dgTaiKhoan.ItemsSource = taiKhoans;
        }

        private void btnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn xóa các bản ghi đã được chọn?",
                                          "Xóa bản ghi",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                removeSelectItem();
            }
        }

        private BackgroundWorker removeItem = new BackgroundWorker();
        private void removeSelectItem()
        {
            var selectSet = new HashSet<int>();
            var rItems = new List<ThongTinTaiKhoan>();
            foreach (ThongTinTaiKhoan item in dgTaiKhoan.SelectedItems)
            {
                rItems.Add(item);
                selectSet.Add(item.ID);
            }
            foreach (var item in rItems)
            {
                taiKhoans.Remove(item);
            }

            var items = new List<TaiKhoanFacebook>();
            foreach (var tk in objectList)
            {
                if (selectSet.Contains(tk.IDTaiKhoanF))
                {
                    items.Add(tk);
                }
            }

            removeItem.RunWorkerAsync(items);
        }
    }
}
