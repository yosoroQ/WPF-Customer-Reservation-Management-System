using _05SQL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CMS.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
/*        public List<Customer> Customers { get; set; } = new();*/

        //分解 —— 嵌套子视图模型（生成类型）

        //ObservableCollection代替List
        public ObservableCollection<CustomerViewModel> Customers { get; set; } = new();
        /*public ObservableCollection<AppointmentViewModel> Appointments { get; set; } = new();*/
        public ObservableCollection<DateTime> Appointments { get; set; } = new();

        //选择客户
        private CustomerViewModel _selectedCustomer;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate { get =>_selectedDate;
            set
            {
                if (_selectedDate != value)
                { 
                    _selectedDate= value;
                    RaisePropertyChange(nameof(SelectedDate));
                }
            }
        }

        /*        public CustomerViewModel Sele1ctedCustomer
                {
                    get => _selectedCustomer; set
                    {
                        if (value != _selectedCustomer)
                        {
                            _selectedCustomer = value;
                            RaisePropertyChange(nameof(SelectedCustomer));

                            //在SelectedCustomer方法中调用LoadAppointments方法
                            LoadAppointments(SelectedCustomer.Id);
                        }
                    }
                }*/

        public CustomerViewModel SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (value != _selectedCustomer)
                {
                    _selectedCustomer = value;
                    RaisePropertyChange(nameof(SelectedCustomer));
                    LoadAppointments(SelectedCustomer.Id);
                }
            }
        }

        public void LoadCustomers()
        {
            Customers.Clear();
            using (var db = new AppDbContext())
            {

                var customers = db.Customers
                    .Include(c => c.Appointments)
                    .ToList();

                foreach (var c in customers)
                {
                    Customers.Add(new CustomerViewModel(c));
                }
            }
        }

        public void ClearSelectedCustomer()
        {
            _selectedCustomer = null;
            RaisePropertyChange(nameof(Customers));
        }

        public void SaveCustomer(string name, string idNumber, string address)
        {
            if (SelectedCustomer != null)
            {
                //更新客户数据
                using (var db = new AppDbContext())
                {
                    var customer = db.Customers.Where(c => c.Id == SelectedCustomer.Id).FirstOrDefault();
                    customer.Name = name;
                    customer.IdNumber = idNumber;
                    customer.Address = address;
                    db.SaveChanges();

                }
            }
            else
            {
                //添加新客户
                using (var db = new AppDbContext())
                {
                    var newCustomer = new Customer()
                    {
                        Name = name,
                        IdNumber = idNumber,
                        Address = address,
                    };
                    db.Customers.Add(newCustomer);
                    db.SaveChanges();

                }
                //此处需在LoadCustomers方法中新增Customers.Clear()方法
                LoadCustomers();
            }
        }

        public void LoadAppointments(int customerId)
        {
            Appointments.Clear();
            using (var db = new AppDbContext())
            {

                var appointments = db.Appointments.Where(a => a.CustomerId == customerId).ToList();

                foreach (var a in appointments)
                {
                    Appointments.Add(a.Time);
                }
            }
        }

        public void AddAppointment()
        {
            if (SelectedCustomer == null)
            {
                return;
            }

            using (var db = new AppDbContext())
            {

                var newAppointment = new Appointment()
                {
                    Time = SelectedDate.Value,
                    CustomerId = SelectedCustomer.Id
                };
                db.Appointments.Add(newAppointment);
                db.SaveChanges();
            }
            SelectedDate = null;
            LoadAppointments(SelectedCustomer.Id);
        }
    }
}


/*using _05SQL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CMS.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //public List<Customer> Customers { get; set; } = new();
        public ObservableCollection<CustomerViewModel> Customers { get; set; } = new();
        public List<AppointmentViewModel> Appointments { get; set; } = new();

        private CustomerViewModel _selectedCustomer;

        public CustomerViewModel SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (value != _selectedCustomer)
                {
                    _selectedCustomer = value;
                    RaisePropertyChanged(nameof(SelectedCustomer));
                }
            }
        }

        public void LoadCustomers()
        {
            Customers.Clear();
            using (var db = new AppDbContext())
            {
                // Select * from Customers as c join Appointments as a on c.Id = a. CustomerId
                var customers = db.Customers
                    //.Include(c => c.Appointments)
                    .ToList();

                foreach (var c in customers)
                {
                    Customers.Add(new CustomerViewModel(c));
                }
            }
        }

        public void ClearSelectedCustomer()
        {
            _selectedCustomer = null;
            RaisePropertyChanged(nameof(SelectedCustomer));
        }

        public void SaveCustomer(string name, string idNumber, string address)
        {
            if (SelectedCustomer != null)
            {
                // 更新客户数据
                using (var db = new AppDbContext())
                {
                    var customer = db.Customers.Where(c => c.Id == SelectedCustomer.Id).FirstOrDefault();
                    customer.Name = name;
                    customer.IdNumber = idNumber;
                    customer.Address = address;
                    db.SaveChanges();
                }
            }
            else
            {
                // 添加新客户
                using (var db = new AppDbContext())
                {
                    var newCustomer = new Customer()
                    {
                        Name = name,
                        IdNumber = idNumber,
                        Address = address
                    };
                    db.Customers.Add(newCustomer);
                    db.SaveChanges();
                }
                LoadCustomers();
            }
        }
    }
}
*/