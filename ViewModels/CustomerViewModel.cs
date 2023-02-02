using _05SQL.Models;

namespace WPF_CMS.ViewModels
{
    public class CustomerViewModel
    {
        private Customer _customer;
        public CustomerViewModel(Customer customer)
        {
            _customer= customer;
        }

        public int Id { get => _customer.Id; }
        public string Name { get => _customer.Name;
            set
            {
                if (_customer.Name != value)
                { 
                    _customer.Name = value;
                }
            }
        }

        public string IdNumber
        {
            get => _customer.IdNumber;
            set
            {
                if (_customer.IdNumber != value)
                {
                    _customer.IdNumber = value;
                }
            }
        }

        public string Address
        {
            get => _customer.Address;
            set
            {
                if (_customer.Address != value)
                {
                    _customer.Address = value;
                }
            }
        }
    }
}