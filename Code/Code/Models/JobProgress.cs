using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Models
{
    public class JobProgress : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public JobProgress(int ht, int ts, string title = "Một công việc")
        {
            this.ht = ht;
            this.ts = ts;
            this.title = title;
        }

        private int ht;
        private int ts;

        public int HT
        {
            get
            {
                return this.ht;
            }
            set
            {
                this.ht = value;
                this.OnPropertyChanged("HT");
            }
        }
        public int TS
        {
            get
            {
                return this.ts;
            }
            set
            {
                this.ts = value;
                this.OnPropertyChanged("TS");
            }
        }


        private string title;

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
                this.OnPropertyChanged("Title");
            }
        }

    }
}
