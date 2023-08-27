using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MemberManager.Model
{
    class Member : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChenaged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
