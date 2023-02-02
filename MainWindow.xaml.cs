using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using WPF_CMS.ViewModels;

namespace WPF_CMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();

            _viewModel.LoadCustomers();

            DataContext = _viewModel;
            //ShowCustomers();
        }

        //添加客户
        private void ClearSelectedCustomer_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearSelectedCustomer();
        }

        private void SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = NameTextBox.Text.Trim();
                string idNumber = IdNumberTextBox.Text.Trim();
                string address = AddressTextBox.Text.Trim();

                _viewModel.SaveCustomer(name,idNumber,address);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.AddAppointment();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        //private void ShowCustomers()
        //{
        //    try
        //    {
        //        using (var db = new AppDbContext())
        //        {
        //            var customers = db.Customers.ToList();
        //            customerList.DisplayMemberPath = "Name";
        //            customerList.SelectedValuePath = "Id";
        //            customerList.ItemsSource = customers;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.ToString());
        //    }
        //}

        //private void customerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        Customer selectedItem = customerList.SelectedItem as Customer;
        //        if (selectedItem == null)
        //        {
        //            appointmentList.ItemsSource = null;
        //            return;
        //        }
        //        NameTextBox.Text = selectedItem.Name;
        //        IdTextBox.Text = selectedItem.IdNnumber;
        //        AddressTextBox.Text = selectedItem.Address;

        //        using (var db = new AppDbContext())
        //        {
        //            var customerId = customerList.SelectedValue;
        //            var appointment = db.Appointments.Where(a => a.CustomerId == (int)customerId).ToList();

        //            appointmentList.DisplayMemberPath = "Time";
        //            appointmentList.SelectedValuePath = "Id";
        //            appointmentList.ItemsSource = appointment;
        //        }

        //    }
        //    catch (Exception error)
        //    {
        //        MessageBox.Show(error.ToString());
        //    }
        //}

        //private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var appointmentId = appointmentList.SelectedValue;

        //        using (var db=new AppDbContext())
        //        {
        //            var appointmentToRmove = db.Appointments.Where(a => a.Id == (int)appointmentId).FirstOrDefault();

        //            db.Appointments.Remove(appointmentToRmove);

        //            db.SaveChanges();
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        MessageBox.Show(error.ToString());
        //    }
        //    finally
        //    {
        //        customerList_SelectionChanged(null, null);
        //    }
        //}

        //private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var customerId = customerList.SelectedValue;
        //        using (var db = new AppDbContext())
        //        {
        //            var customerToRemove = db.Customers
        //                .Include(c => c.Appointments)
        //                .Where(c => c.Id == (int)customerId)
        //                .FirstOrDefault();
        //            db.Customers.Remove(customerToRemove);
        //            db.SaveChanges();
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        MessageBox.Show(error.ToString());
        //    }
        //    finally
        //    {
        //        ShowCustomers();
        //        customerList_SelectionChanged(null, null);
        //    }
        //}

        //private void AddCustomer_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        using (var db = new AppDbContext())
        //        {
        //            var customer = new Customer()
        //            {
        //                Name = NameTextBox.Text,
        //                IdNnumber = IdTextBox.Text,
        //                Address = AddressTextBox.Text
        //            };

        //            db.Customers.Add(customer);
        //            db.SaveChanges();
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        MessageBox.Show(error.ToString());
        //    }
        //    finally
        //    {
        //        ShowCustomers();
        //    }
        //}

        //private void AddAppointment_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        using (var db = new AppDbContext())
        //        {
        //            var appointment = new Appointment()
        //            {
        //                Time = DateTime.Parse(AppointmentDatePicker.Text),
        //                CustomerId = (int)customerList.SelectedValue
        //            };

        //            db.Appointments.Add(appointment);
        //            db.SaveChanges();
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        MessageBox.Show(error.ToString());
        //    }
        //    finally
        //    {
        //        customerList_SelectionChanged(null, null);
        //    }
        //}

        //private void UpdateCustomer_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        using (var db=new AppDbContext())
        //        {
        //            var customer = db.Customers.Where(c => c.Id == (int)customerList.SelectedValue).FirstOrDefault();

        //            customer.Name = NameTextBox.Text.Trim();
        //            customer.IdNnumber = IdTextBox.Text.Trim();
        //            customer.Address = AddressTextBox.Text.Trim();

        //            db.SaveChanges();
        //        }

        //    }
        //    catch (Exception error)
        //    {
        //        MessageBox.Show(error.ToString());
        //    }
        //    finally
        //    {
        //        ShowCustomers();
        //    }
        //}
    }
}
