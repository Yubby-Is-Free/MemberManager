using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Data;
using System.Windows;

namespace MemberManager.ViewModel
{

    class MemberViewModel : INotifyPropertyChanged
    {
        DB.DataBaseAgent agent = new DB.DataBaseAgent();

        
        private string add_name;
        private string add_age;
        private string add_phone;

        //조회 TextBox와 추가 TextBox는 다른 항목인데 같은 값을 바인딩 할 경우 의도치 않게 TextBox의 값이 바뀌는걸 방지.
        //데이터 조회를 위해 TextBox에 입력되는 데이터가 바인딩 될 변수
        public string Name { get; set; }
        public string Phone { get; set; }

        //데이터 추가를 위해 TextBox에 입력되는 데이터가 바인딩 될 변수
        public string AddName { get { return add_name; } set { add_name = value; OnPropertyChenaged("AddName"); } }
        public string AddAge { get { return add_age; } set { add_age = value; OnPropertyChenaged("AddAge"); } }
        public string AddPhone { get { return add_phone; } set { add_phone = value; OnPropertyChenaged("AddPhone"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChenaged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private ObservableCollection<Model.Member> _mem;

        public ObservableCollection<Model.Member> mem
        {
            get { return _mem; }
            set
            {
                _mem = value;
                OnPropertyChenaged("mem");
            }
        }

        private Model.Member _selectedMember;
        public Model.Member selectedMember
        {
            get { return _selectedMember; }
            set
            {
                _selectedMember = value;
                OnPropertyChenaged("selectedMember");
            }
        }

        public MemberViewModel()
        {
            InitMembers();
        }

        public ICommand Search
        {
            get { return new DelegateCommand(SearchMembers); }
        }

        public ICommand Delete
        {
            get { return new DelegateCommand(DeleteMembers); }
        }

        public ICommand Add
        {
            get { return new DelegateCommand(AddMember); }
        }

        private void InitMembers()
        {
            _mem = new ObservableCollection<Model.Member>();

            foreach(DataRow row in agent.GetMember("", ""))
            {
                _mem.Add(new Model.Member { Name = row["Name"].ToString(), Age = int.Parse(row["Age"].ToString()), Phone = row["Phone"].ToString() });
            }
        }

        private void SearchMembers()
        {
            _mem = new ObservableCollection<Model.Member>();

            foreach (DataRow row in agent.GetMember(Name == null ? "" : Name, Phone == null ? "" : Phone))
            {
                _mem.Add(new Model.Member { Name = row["Name"].ToString(), Age = int.Parse(row["Age"].ToString()), Phone = row["Phone"].ToString() });
            }

            System.Threading.Thread.Sleep(1000);

            mem = _mem;
        }

        private void DeleteMembers()
        {
            if(agent.DeleteMember(_selectedMember.Name, _selectedMember.Phone))
            {
                SearchMembers();

                System.Threading.Thread.Sleep(1000);

                mem = _mem;
            }
            else
            {
                MessageBox.Show("삭제를 실패하였습니다.");
            }
        }
        
        private void AddMember()
        {
            if(AddName == "" || AddPhone == "" || AddAge == "") 
            {
                MessageBox.Show("모든 항목을 입력해주세요.");
                return;
            }

            try
            {
                int.Parse(AddAge);
            }
            catch
            {
                MessageBox.Show("나이는 숫자로 입력해주세요.");
                return;
            }

            if (agent.AddMember(AddName, int.Parse(AddAge), AddPhone))
            {
                AddName = "";
                AddAge = "";
                AddPhone = "";
                
                SearchMembers();

                System.Threading.Thread.Sleep(1000);

                mem = _mem;
            }
            else
            {
                MessageBox.Show("추가를 실패하였습니다.");
            }
        }
    }

    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action execute;

        public DelegateCommand(Action exectue) : this(exectue, null)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }
            return this.canExecute();
        }

        public void Execute(object parameter)
        {
            this.execute();
        }
    }
}
