using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Code.Utils;
using Code.Utils.Story;
using Code.Views;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Security.Principal;
using Code.Models;
using System.Windows.Markup;

namespace Code.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ThietBi thietBi;
        private List<string> ThietBiDuocDung = new List<string>();
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class DeviceStatus : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private int stt;
            private string deviceId;
            private TrangThai status;
            private string url;

            public class TrangThai
            {
                protected readonly string name;
                public TrangThai(string name)
                {
                    this.name = name;
                }

                public override string ToString()
                {
                    return this.name;
                }

                public static TrangThai DANG_CHAY = new TrangThai("Đang chạy");
                public static TrangThai THANH_CONG = new TrangThai("Thành công");
                public static TrangThai THAT_BAI = new TrangThai("Thất bại");
                public static TrangThai KHONG_HO_TRO = new TrangThai("Thiết bị không thể thực hiện hành động");
            }
            public int Stt
            {
                get
                {
                    return this.stt;
                }
                set
                {
                    this.stt = value;
                    this.OnPropertyChanged("Stt");
                }
            }
            public string Url
            {
                get
                {
                    return this.url;
                }
                set
                {
                    this.url = value;
                    this.OnPropertyChanged("Url");
                }
            }
            public TrangThai Status
            {
                get
                {
                    return this.status;
                }
                set
                {
                    this.status = value;
                    this.OnPropertyChanged("Status");
                }
            }
            public string DeviceId
            {
                get
                {
                    return this.deviceId;
                }
                set
                {
                    this.deviceId = value;
                    this.OnPropertyChanged("DeviceId");
                }
            }
        }

        protected List<Thread> threads = new List<Thread>();
        public ObservableCollection<DeviceStatus> devices = new ObservableCollection<DeviceStatus>();
        protected Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;
        protected List<int> soLanDaLapCV = new List<int>();

        public ICommand ShowPopUpWindowCommand { get; }
        public ICommand StopAction { get; }

        protected ViewModelBase()
        {
            thietBi = ThietBi.GetInstance();
            ShowPopUpWindowCommand = new ViewModelCommand(ExecuteShowPopUpWindow);
            StopAction = new ViewModelCommand(StopActionAndDeleteStatus);
        }

        protected PopupChonThietBiView popupChonThietBiView;

        protected virtual void ThemCongViec(List<string> thietbi)
        {
            foreach (var tb in thietbi)
            {
                this.thietBi.setUse(tb, true);
                this.ThietBiDuocDung.Add(tb);
            }
            var index = this.threads.Count;
            var thread = new Thread(new ThreadStart(() =>
            {
                QuanLyCongViecChoCacThietBi(thietbi,long.MaxValue, index);
            }));
            soLanDaLapCV.Add(0);
            thread.Start();
            this.threads.Add(thread);
        }

        protected virtual void ExecuteShowPopUpWindow(object obj)
        {
            popupChonThietBiView?.Close();
            popupChonThietBiView = new PopupChonThietBiView(ThemCongViec);
            popupChonThietBiView.Show();
        }

        protected virtual void ExecuteStopAction(object obj)
        {
            foreach (Thread thread in threads)
            {
                thread.Interrupt();
                thread.Abort();
            }
            foreach (var tb in this.ThietBiDuocDung)
            {
                thietBi.setUse(tb, false);
            }
            ThietBiDuocDung = new List<string>();
            this.threads = new List<Thread>();
            this.soLanDaLapCV.Clear();
        }
        protected virtual void QuanLyCongViecChoCacThietBi(List<string> thietbi, long soLanLap, int indexInGThreads)
        {
            List<Thread> jobThreads = new List<Thread>();
            foreach (var tb in thietbi)
            {
                jobThreads.Add(null);
            }
            try
            {
                var url = getCurrentUrl().ToString();
                int nextIndex = 0;
                while (soLanDaLapCV[indexInGThreads] < soLanLap)
                {
                    if (nextIndex != -1)
                    {
                        if (!this.thietBi.danhSachThietBi.Contains(thietbi[nextIndex]))
                        {
                            thietBi.setUse(thietbi[nextIndex], false);
                            thietbi.RemoveAt(nextIndex);
                            jobThreads.RemoveAt(nextIndex);
                            nextIndex = -1;
                            continue;
                        }
                        jobThreads[nextIndex] = new Thread(new ThreadStart(() =>
                        {
                            var tb = thietbi[nextIndex];
                            if (!ThucHienCongViecTrenThietBi(tb, url, indexInGThreads))
                            {
                                var ind = thietbi.IndexOf(tb);
                                thietBi.setUse(tb, false);
                                thietbi.RemoveAt(ind);
                                jobThreads.RemoveAt(ind);
                            }
                        }));
                        jobThreads[nextIndex].Start();
                    }
                    Thread.Sleep(2000);
                    nextIndex = -1;
                    for (int i = 0; i < jobThreads.Count; i++)
                    {
                        if (jobThreads[i] == null || !jobThreads[i].IsAlive)
                        {
                            nextIndex = i;
                            break;
                        }
                    }

                    if (jobThreads.Count == 0)
                    {
                        break;
                    }
                }
                foreach (var tb in this.ThietBiDuocDung)
                {
                    thietBi.setUse(tb, false);
                }
                ThietBiDuocDung = new List<string>();
            }
            catch (Exception)
            {
                foreach (var thread in jobThreads)
                {
                    thread?.Abort();
                }
            }
        }

        private int _thanhCong;
        public int ThanhCong
        {
            get { return this._thanhCong; }
            set
            {
                this._thanhCong = value;
                OnPropertyChanged("ThanhCong");
            }
        }

        private int _thatBai;
        public int ThatBai
        {
            get { return this._thatBai; }
            set
            {
                this._thatBai = value;
                OnPropertyChanged("ThatBai");
            }
        }

        Mutex mutexThanhCong = new Mutex();
        protected virtual void TangThanhCong(int threadIndex)
        {
            mutexThanhCong.WaitOne();
            soLanDaLapCV[threadIndex]++;
            ++ThanhCong;
            mutexThanhCong.ReleaseMutex();
        }

        Mutex mutexThatBai = new Mutex();
        protected virtual void TangThatBai(int threadIndex)
        {
            mutexThatBai.WaitOne();
            ++ThatBai;
            mutexThatBai.ReleaseMutex();
        }

        private Mutex themDongChoBangMutex = new Mutex();

        protected virtual bool ThucHienCongViecTrenThietBi(string idThietBi, string url, int indexInGThread)
        {
            var job = createScriptToRun(idThietBi, url);
            DeviceStatus status;
            if (job == null)
            {
                themDongChoBangMutex.WaitOne();
                status = new DeviceStatus { Stt = devices.Count + 1, DeviceId = idThietBi, Status = DeviceStatus.TrangThai.KHONG_HO_TRO, Url = url };
                mainDispatcher.Invoke(() =>
                {
                    devices.Add(status);
                });
                themDongChoBangMutex.ReleaseMutex();
                return false;
            }
            themDongChoBangMutex.WaitOne();
            status = new DeviceStatus { Stt = devices.Count + 1, DeviceId = idThietBi, Status = DeviceStatus.TrangThai.DANG_CHAY, Url = url };
            mainDispatcher.Invoke(() =>
            {
                devices.Add(status);
            });
            themDongChoBangMutex.ReleaseMutex();
            try
            {
                job.onTitleChange = (title) =>
                {
                    status.Status = new DeviceStatus.TrangThai(title);
                };

                if (job.RunScript())
                {
                    status.Status = DeviceStatus.TrangThai.THANH_CONG;
                    TangThanhCong(indexInGThread);
                }
                else
                {
                    status.Status = DeviceStatus.TrangThai.THAT_BAI;
                    TangThatBai(indexInGThread);
                }
            }
            catch (AdbException ex)
            {
                status.Status = DeviceStatus.TrangThai.THAT_BAI;
                job.OnTerminateOrPause();
                Console.WriteLine(ex.Message);
                TangThatBai(indexInGThread);
            }
            catch (Exception ex)
            {
                status.Status = DeviceStatus.TrangThai.THAT_BAI;
                job.OnTerminateOrPause();
                TangThatBai(indexInGThread);
            }
            return true;
        }

        protected virtual string getCurrentUrl()
        {
            return "";
        }

        protected virtual BaseScript createScriptToRun(string thietbiId, string url)
        {
            return new BaseScript();
        }

        protected string RandomString(int length = 10)
        {
            var allowChars = "qweryuiopasdfghjklzxcvbnm";
            var random = new Random();
            var str = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var index = random.Next(allowChars.Length);
                str.Append(allowChars[index]);
            }
            return str.ToString();
        }

        private void StopActionAndDeleteStatus(object obj)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn dừng tất cả các công việc đang thực thi (nếu có) ?",
                                            "Dừng hành động",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteStopAction(obj);
            }
        }
    }
}
