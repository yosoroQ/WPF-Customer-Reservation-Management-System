# C# - WPF实战-重构与美化

## 本章目标
* 构建数据模型（Model）
* 重构项目：MVVM （Model-View-ViewModel）
* 美化UI（Material Design）

# 什么是数据模型

## Model

* Model：一种可以描述复杂事物的方法。

## 逻辑概念 or 物理概念

* ![image-20230131222641332](http://qny.expressisland.cn/schoolOpens/image-20230131222641332.png)

# 构建数据模型

## 重构：构建数据模型

* 逆向数据库获得数据模型（Model）。
* 使用Entity Framework取代SQL语句。
* 通过数据模型向UI传递和绑定数据。

## 所需工具

* ![image-20230131222826671](http://qny.expressisland.cn/schoolOpens/image-20230131222826671.png)

## 安装nuget程序包

### Microsoft.EntityFrameworkCore.SqlServer

### Microsoft.EntityFrameworkCore.Tools

![image-20230131223647541](C:/Users/one/AppData/Roaming/Typora/typora-user-images/image-20230131223647541.png)

## 在项目中新建Models文件夹

![image-20230131224302066](http://qny.expressisland.cn/schoolOpens/image-20230131224302066.png)

## 打开程序包管理器控制台

![image-20230131224233621](http://qny.expressisland.cn/schoolOpens/image-20230131224233621.png)

## 输入命令

```powershell
Scaffold-DbContext "Data Source=DESKTOP-V6GQHHS\SQLSERVER2012;Initial Catalog=master;Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context AppDbContext
```

## 得到逆向数据库得到的数据模型

![image-20230131225618732](http://qny.expressisland.cn/schoolOpens/image-20230131225618732.png)

# ORM数据管理（上）

* 不用写SQL了。

## 修改原有代码

### （showCustomers）

### （customerList_SelectionChanged）

```c#
namespace _05SQL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection _sqlConnection;

        public MainWindow()
        {
            InitializeComponent();
            string connectionString =
                "Data Source=DESKTOP-V6GQHHS\\SQLSERVER2012;Initial Catalog=master;Integrated Security=True";

            _sqlConnection = new SqlConnection(connectionString);

            showCustomers();
        }
        private void showCustomers()
        {
            try
            {
                using (var db = new AppDbContext()) 
                {
                    var customers = db.Customers.ToList();
                    customerList.DisplayMemberPath = "Name";
                    customerList.SelectedValuePath = "Id";
                    customerList.ItemsSource = customers;
                }
            }
            catch(Exception e) { 
                MessageBox.Show(e.ToString());
            }    
        }


        //显示关联型数据：客户预约记录
        private void customerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Customer selectedItem = customerList.SelectedItem as Customer;
                NameTextBox.Text = selectedItem.Name;
                IdTextBox.Text = selectedItem.IdNumber;
                AddressTextBox.Text = selectedItem.Address;

                using (var db = new AppDbContext())
                {
                    var customerId = customerList.SelectedValue;
                    var appointment = db.Appointments.Where(a => a.CustomerId == (int)customerId).ToList();

                    appointmentList.DisplayMemberPath = "Time";
                    appointmentList.SelectedValuePath = "Id";
                    appointmentList.ItemsSource = appointment;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
```

## 运行测试

![image-20230131232051905](http://qny.expressisland.cn/schoolOpens/image-20230131232051905.png)

# ORM数据管理（下）

## 实例

### DeleteAppointment_Click

### DeleteCustomer_Click

### AddCustomer_Click

### AddAppointment_Click

### UpdateCustomer_Click

```c#
private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var appointmentId = appointmentList.SelectedValue;
                using (var db = new AppDbContext())
                {
                    var appointmentToRemove = db.Appointments.Where
                        (a => a.Id == (int)appointmentId).FirstOrDefault();

                    db.Appointments.Remove(appointmentToRemove);
                    db.SaveChanges();
                
                }

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally {
                customerList_SelectionChanged(null, null);
            }
}

        //删除客户
        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            try {
                var customerId = customerList.SelectedValue;
                using (var db = new AppDbContext())
                {
                    var customerToRemove = db.Customers
                        .Include(c => c.Appointments)
                        .Where(c => c.Id == (int)customerId).FirstOrDefault();

                    db.Customers.Remove(customerToRemove);
                    db.SaveChanges();
                }

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                showCustomers();
                customerList_SelectionChanged(null, null);
            }
        }

        //添加客户
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            try {
                using (var db = new AppDbContext())
                {
                    var customer = new Customer()
                    {
                        Name = NameTextBox.Text,
                        IdNumber = IdTextBox.Text,
                        Address = AddressTextBox.Text
                    };

                    db.Customers.Add(customer);
                    db.SaveChanges();
                }

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                showCustomers();
            }
        }

        //预约
        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var appointment = new Appointment()
                    {
                        Time = DateTime.Parse(AppointmentDatePicker.Text),
                        CustomerId = (int)customerList.SelectedValue
                    };

                    db.Appointments.Add(appointment);
                    db.SaveChanges();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                customerList_SelectionChanged(null,null);
            }
        }

        //更新客户数据
        private void UpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            try {
                using (var db = new AppDbContext())
                {
                       var customer = db.Customers.Where
                       (c => c.Id == (int)customerList.SelectedValue).FirstOrDefault();

                    customer.Name = NameTextBox.Text.Trim();
                    customer.IdNumber = IdTextBox.Text.Trim();
                    customer.Address = AddressTextBox.Text.Trim();

                    db.SaveChanges();
                }

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                showCustomers();
            }
        }
```

#### 修改完DeleteCustomer_Click方法后报错，需修改customerList_SelectionChanged中的`selectedItem`

```c#
                //---------------------------------------      

//显示关联型数据：客户预约记录
        private void customerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Customer selectedItem = customerList.SelectedItem as Customer;
                //新增if判断
                if (selectedItem == null)
                {
                    appointmentList.ItemsSource = null;
                    return;
                }
                                //新增if判断
                NameTextBox.Text = selectedItem.Name;
                }
                //---------------------------------------
```

## 删除所有与DbConnection相关的代码

### MainWindow

* 注释的即与DbConnection的代码

```c#
        public MainWindow()
        {
            InitializeComponent();
/*            string connectionString =
                "Data Source=DESKTOP-V6GQHHS\\SQLSERVER2012;Initial Catalog=master;Integrated Security=True";

            _sqlConnection = new SqlConnection(connectionString);
*/
            showCustomers();
        }
```

### 删除各个方法中的_sqlConnection.Close();

# 美化主页面

```xaml
<Window x:Class="WPF_CMS.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WPF_CMS"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition />
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="240"/>
            <ColumnDefinition Width="280"/>
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        
        <!--header-->
        <Border Grid.ColumnSpan="3" Background="#7f3089">
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Center">
                <Image Height="90" Margin="5" Source="/Images/logo.jpg"/>
                <TextBlock Text="WPF客户管理系统" FontSize="40" VerticalAlignment="Center" Foreground="#ffffff"/>
            </StackPanel>
        </Border>

        <StackPanel Grid.Row="1" Grid.Column="0">
            <Button Content="添加客户"/>
            <ListView />
        </StackPanel>

        <StackPanel Grid.Row="1" Grid.Column="1">
            <TextBlock Text="姓名" Margin="10 10 10 0"/>
            <TextBox Margin="10" />
            <TextBlock Text="身份证" Margin="10 10 10 0"/>
            <TextBox Margin="10" />
            <TextBlock Text="地址" Margin="10 10 10 0"/>
            <TextBox Margin="10" />
            <Button Content="保存" Margin="10 10 10 30" VerticalAlignment="Bottom" HorizontalAlignment="Left" />
        </StackPanel>

        <StackPanel Grid.Row="1" Grid.Column="2">
            <ListView />
            <TextBlock Text="添加新预约" />
            <DatePicker Margin="10" />
            <Button Content="预约" />
        </StackPanel>
        
    </Grid>
</Window>
```

![image-20230201134217125](http://qny.expressisland.cn/schoolOpens/image-20230201134217125.png)

# 组件化布局

## 新建Controls文件夹

### 新建用户控件（WPF）

![image-20230201134629484](http://qny.expressisland.cn/schoolOpens/image-20230201134629484.png)

### HeaderControl.xaml

```xaml
<UserControl x:Class="WPF_CMS.Controls.HeaderControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
             xmlns:local="clr-namespace:WPF_CMS.Controls"
             mc:Ignorable="d" 
             d:DesignHeight="450" d:DesignWidth="800">
    <!--header-->
    <Border  Grid.ColumnSpan="3" Background="#7f3089">
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Center">
            <Image Height="90" Margin="5" Source="/Images/logo.jpg"/>
            <TextBlock Text="WPF客户管理系统" FontSize="40" VerticalAlignment="Center" Foreground="#ffffff"/>
        </StackPanel>
    </Border>
</UserControl>
```

* 在MainWindow.xaml中仅需如下代码所示即可实现调用。

```xaml
        <!--header-->
   <controls:HeaderControl Grid.ColumnSpan="3"/>
```

## 测试

![image-20230201135236779](http://qny.expressisland.cn/schoolOpens/image-20230201135236779.png)

# MVVM架构

## 什么是MVVM

![image-20230201135329118](http://qny.expressisland.cn/schoolOpens/image-20230201135329118.png)

![image-20230201135432481](http://qny.expressisland.cn/schoolOpens/image-20230201135432481.png)

![image-20230201135624656](http://qny.expressisland.cn/schoolOpens/image-20230201135624656.png)

![image-20230201135710192](http://qny.expressisland.cn/schoolOpens/image-20230201135710192.png)

## MVVM的优点

* 兼容MVC架构。
* 方便测试 和 维护。

## MVVM的缺点

* 代码量增加。
* 对象调用复杂度增加。

# 创建视图模型（显示客户列表）

## 新建ViewModels文件夹

### 建立MainViewModel类

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WPF_CMS.Models;

namespace WPF_CMS.ViewModels
{
    public class MainViewModel
    {
        public List<Customer> Customers { get; set;} = new();
        public void LoadCustomers()
        {
            using (var db = new AppDbContext())
            {
                Customers = db.Customers.Include(c => c.Appointments).ToList();
            }
        }
    }
}

```

## 在MainWindow.xaml.cs中声明私有的视图模型

```c#
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
            }
}
```

## 修改MainWindow.xaml —— 显示客户列表

```xaml
        <!--header-->
        <controls:HeaderControl Grid.ColumnSpan="3"/>

        <StackPanel Grid.Row="1" Grid.Column="0">
            <Button Content="添加客户"/>
            <ListView ItemsSource="{Binding Customers,Mode=OneWay}" DisplayMemberPath="Name"/>
        </StackPanel>
```

## 测试

### 正常显示用户列表

![image-20230201222511590](http://qny.expressisland.cn/schoolOpens/image-20230201222511590.png)

# 双向绑定（选择客户）

## MainViewModel类

```c#
using _05SQL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CMS.ViewModels
{
    public class MainViewModel
    {
        public List<Customer> Customers { get; set; } = new();

        //选择客户
        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer; set
            {
                if (value != _selectedCustomer)
                {
                    _selectedCustomer = value;
                }
            }
        }

        public void LoadCustomers()
        {
            using (var db = new AppDbContext())
            {
                Customers = db.Customers.Include(c => c.Appointments).ToList();
            }
        }
    }
}

```

## 修改MainWindow.xaml —— 添加客户

```xaml
        <!--header-->
        <controls:HeaderControl Grid.ColumnSpan="3"/>

        <StackPanel Grid.Row="1" Grid.Column="0">
            <Button Content="添加客户"/>
            <ListView ItemsSource="{Binding Customers, Mode=OneWay}" DisplayMemberPath="Name" SelectedItem="{Binding SelectedCustomer, Mode=TwoWay}" />
        </StackPanel>

        <StackPanel Grid.Row="1" Grid.Column="1">
            <TextBlock Text="姓名" Margin="10 10 10 0"/>
            <TextBox Margin="10" Text="{Binding SelectedCustomer.Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
            <TextBlock Text="身份证" Margin="10 10 10 0"/>
            <TextBox Margin="10" Text="{Binding SelectedCustomer.IdNumber, Mode=TwoWay}" />
            <TextBlock Text="地址" Margin="10 10 10 0"/>
            <TextBox Margin="10" Text="{Binding SelectedCustomer.Address, Mode=TwoWay}" />
            <Button Content="保存" Margin="10 10 10 30" VerticalAlignment="Bottom" HorizontalAlignment="Left" />
        </StackPanel>

        <StackPanel Grid.Row="1" Grid.Column="2">
            <ListView ItemsSource="{Binding SelectedCustomer.Appointments, Mode=TwoWay}" />
            <TextBlock Text="添加新预约" />
            <DatePicker Margin="10" />
            <Button Content="预约" />
        </StackPanel>
```

# ViewModel的嵌套与分解

## 注释MainViewModel类的 .Include(c => c.Appointments)

```c#
        public void LoadCustomers()
        {
            using (var db = new AppDbContext())
            {
                Customers = db.Customers
/*                    .Include(c => c.Appointments)*/
                    .ToList();
            }
        }
    }
```

## 分解 —— 嵌套子视图模型（生成类型）

### MainViewModel.cs

```c#
using _05SQL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CMS.ViewModels
{
    public class MainViewModel
    {
        /*        public List<Customer> Customers { get; set; } = new();*/

        //分解 —— 嵌套子视图模型（生成类型）
        public List<CustomerViewModel> Customers { get; set; } = new();
        public List<AppointmentViewModel> Appointments { get; set; } = new();

        //选择客户
        private CustomerViewModel _selectedCustomer;
        public CustomerViewModel SelectedCustomer
        {
            get => _selectedCustomer; set
            {
                if (value != _selectedCustomer)
                {
                    _selectedCustomer = value;
                }
            }
        }

        public void LoadCustomers()
        {
            using (var db = new AppDbContext())
            {
                
                var customers = db.Customers
/*                    .Include(c => c.Appointments)*/
                    .ToList();

                foreach (var c in customers)
                {
                    Customers.Add(new CustomerViewModel(c));
                }
            }
        }
    }
}

```

### CustomerViewModel

```c#
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
```

# INotifyPropertyChanged 与 ObservableCollection （新建客户信息）

## 添加客户（MainWindow.xaml.cs）

```c#
        //添加客户
        private void ClearSelectedCustomer_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearSelectedCustomer();
        }
```

## MainViewModel.cs

### ClearSelectedCustomer

```c#
        public void ClearSelectedCustomer()
        {
            _selectedCustomer = null;
        }
```

## INotifyPropertyChanged

### 在MainViewModel.cs中继承INotifyPropertyChanged

* 搜索“PropertyChange”即为新增代码。

```c#
using _05SQL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CMS.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        /*        public List<Customer> Customers { get; set; } = new();*/

        //分解 —— 嵌套子视图模型（生成类型）
        public List<CustomerViewModel> Customers { get; set; } = new();
        public List<AppointmentViewModel> Appointments { get; set; } = new();

        //选择客户
        private CustomerViewModel _selectedCustomer;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        nameof(SelectedCustomer)public CustomerViewModel SelectedCustomer
        {
            get => _selectedCustomer; set
            {
                if (value != _selectedCustomer)
                {
                    _selectedCustomer = value;
                    RaisePropertyChange(nameof(Customers));
                }
            }
        }

        public void LoadCustomers()
        {
            using (var db = new AppDbContext())
            {
                
                var customers = db.Customers
/*                    .Include(c => c.Appointments)*/
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
    }
}
```

## SaveCustomer

```c#
        public void SaveCustomer(string name,string idNumber,string address) {
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
            else {
                //添加新客户
                using (var db = new AppDbContext())
                {
                    var newCustomer = new Customer() {
                        Name = name,
                    IdNumber = idNumber,
                    Address = address,
                };
                db.Customers.Add(newCustomer);
                    db.SaveChanges();

                }; 

               //此处需在LoadCustomers方法中新增Customers.Clear()方法
            LoadCustomers();
            }
        }
    }
```

## MainViewModel.cs

### SaveCustomer_Click

```c#
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
```

### 给TextBox添加Name

```xaml
        <StackPanel Grid.Row="1" Grid.Column="1">
            <TextBlock Text="姓名" Margin="10 10 10 0"/>
            <TextBox Name ="NameTextBox" Margin="10" Text="{Binding SelectedCustomer.Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
            <TextBlock Text="身份证" Margin="10 10 10 0"/>
            <TextBox Name ="IdNumberTextBox" Margin="10" Text="{Binding SelectedCustomer.IdNumber, Mode=TwoWay}" />
            <TextBlock Text="地址" Margin="10 10 10 0"/>
            <TextBox Name ="AddressTextBox" Margin="10" Text="{Binding SelectedCustomer.Address, Mode=TwoWay}" />
            <Button Content="保存" Margin="10 10 10 30" VerticalAlignment="Bottom" HorizontalAlignment="Left" Click="SaveCustomer_Click" />
        </StackPanel>
```

### ObservableCollection

```c#
     //ObservableCollection代替List
        public ObservableCollection<CustomerViewModel> Customers { get; set; } = new();
```

# 显示预约列表

## AppointmentViewModel.cs

```c#
using System;
using _05SQL.Models;

namespace WPF_CMS.ViewModels
{
    public class AppointmentViewModel
    {
        private Appointment _appointment;
        public AppointmentViewModel(Appointment appointment)
        {
            _appointment = appointment;
        }

        public int Id { get => _appointment.Id;  }
        public DateTime Time { get => _appointment.Time;
            set
            {
                if (value != _appointment.Time)
                {
                    _appointment.Time = value;
                }
            }}
    }
}
```

## 修改MainViewModel.cs

```c#
public List<AppointmentViewModel> Appointments { get; set; } = new();
```

* 将上面这段代码修改为以下代码：（`ObservableCollection`）

```c#
public ObservableCollection<AppointmentViewModel> Appointments { get; set; } = new();
```

## MainViewModel.cs —— 创建新方法 —— LoadAppointments

```c#
public void LoadAppointments(int customerId)
        {
            Appointments.Clear();
            using (var db = new AppDbContext())
            {

                var appointments = db.Appointments.Where(a => a.CustomerId == customerId).ToList();

                foreach (var c in appointments)
                {
                    Appointments.Add(new AppointmentViewModel(c));
                }
            }
        }
```

### 在SelectedCustomer方法中调用LoadAppointments方法

```c#
        public CustomerViewModel SelectedCustomer
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
        }
```

## MainWindow.xaml

### DisplayMemberPath="Time"

```xa
        <StackPanel Grid.Row="1" Grid.Column="2">
            <ListView ItemsSource="{Binding Appointments, Mode=TwoWay}" DisplayMemberPath="Time"/>
            <TextBlock Text="添加新预约" />
            <DatePicker Margin="10" />
            <Button Content="预约" />
        </StackPanel>
```

## 测试 —— 显示预约列表

![image-20230202102232702](http://qny.expressisland.cn/schoolOpens/image-20230202102232702.png)

# 添加新预约

## MainViewModel.cs —— 创建新方法 —— AddAppointment

```c#
public void AddAppointment(DateTime selectedDate)
        {
            if (SelectedCustomer == null)
            {
                return;
            }

            using (var db = new AppDbContext())
            {

                var newAppointment = new Appointment()
                {
                    Time = selectedDate,
                    CustomerId = SelectedCustomer.Id
                };
                db.Appointments.Add(newAppointment);
                db.SaveChanges();
            }
            LoadAppointments(SelectedCustomer.Id);
        }
```

## MainWindow.xaml

### Name ="AppointmentDatePicker"

```xaml
        <StackPanel Grid.Row="1" Grid.Column="2">
            <ListView ItemsSource="{Binding Appointments, Mode=TwoWay}" DisplayMemberPath="Time"/>
            <TextBlock Text="添加新预约" />
            <DatePicker Name ="AppointmentDatePicker" Margin="10" />
            <Button Content="预约" Click="AddAppointment_Click" />
        </StackPanel>
```

## MainWindow.xaml.cs创建点击事件

### AddAppointment_Click

```c#
        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime time = DateTime.Parse(AppointmentDatePicker.Text);
                _viewModel.AddAppointment(time);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }
```

# 配置MaterialUI框架

## nuget中下载MaterialDesignThemes

![image-20230202103607892](http://qny.expressisland.cn/schoolOpens/image-20230202103607892.png)

## 项目文档

http://materialdesigninxaml.net/

<img src="http://qny.expressisland.cn/schoolOpens/image-20230202103841939.png" alt="image-20230202103841939" style="zoom:25%;" />

## App.xaml

### 配置MaterialDesignThemes

```xaml
<Application x:Class="_05SQL.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:_05SQL"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.DeepPurple.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.Lime.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

## 测试

**![image-20230202104333292](http://qny.expressisland.cn/schoolOpens/image-20230202104333292.png)**

## 修改主页UI

### 主页名称

```xaml
<Window x:Class="WPF_CMS.MainWindow"
        //-----------------------------省略
        mc:Ignorable="d"
        Title="WPF客户预约管理系统" Height="600" Width="900"
        Background="Transparent" AllowsTransparency="True" WindowStyle="None"
        WindowStartupLocation="CenterScreen" FontFamily="Cambria">
```

### 添加边框

```xaml
<Border Background="White" CornerRadius="30">
     <Grid>
       //------------------------------省略 
    </Grid>
    </Border>
```

### 修改按钮

#### Width="190" Margin="10

```xaml
        <StackPanel Grid.Row="1" Grid.Column="0">
            <Button Content="添加客户" Click="ClearSelectedCustomer_Click" Width="190" Margin="10"/>
            <ListView ItemsSource="{Binding Customers, Mode=OneWay}" DisplayMemberPath="Name" SelectedItem="{Binding SelectedCustomer, Mode=TwoWay}" />
        </StackPanel>
```

```xaml
        <StackPanel Grid.Row="1" Grid.Column="2">
            <ListView ItemsSource="{Binding Appointments, Mode=TwoWay}" DisplayMemberPath="Time"/>
            <TextBlock Text="添加新预约" />
            <DatePicker Name ="AppointmentDatePicker" Margin="10" />
                <Button Content="预约" Click="AddAppointment_Click"  Width="190" Margin="10"/>
        </StackPanel>
```

### 配置MaterialDesign:Card

```xaml
            <MaterialDesign:Card Grid.Row="1" Grid.Column="1"
                                 Width="250" Height="440" Margin="10">
        <StackPanel >
            <TextBlock Text="姓名" Margin="10 10 10 0"/>
            <TextBox Name ="NameTextBox" Margin="10" Text="{Binding SelectedCustomer.Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
            <TextBlock Text="身份证" Margin="10 10 10 0"/>
            <TextBox Name ="IdNumberTextBox" Margin="10" Text="{Binding SelectedCustomer.IdNumber, Mode=TwoWay}" />
            <TextBlock Text="地址" Margin="10 10 10 0"/>
            <TextBox Name ="AddressTextBox" Margin="10" Text="{Binding SelectedCustomer.Address, Mode=TwoWay}" />
            <Button Content="保存" Margin="10 10 10 30" VerticalAlignment="Bottom" HorizontalAlignment="Left" Click="SaveCustomer_Click" />
        </StackPanel>
            </MaterialDesign:Card>

            <MaterialDesign:Card Grid.Row="1" Grid.Column="2" Width="310" Margin="35, 30 35, 30">
            <StackPanel Grid.Row="1" Grid.Column="2">
            <ListView ItemsSource="{Binding Appointments, Mode=TwoWay}" DisplayMemberPath="Time"/>
            <TextBlock Text="添加新预约" />
            <DatePicker Name ="AppointmentDatePicker" Margin="10" />
                <Button Content="预约" Click="AddAppointment_Click"  Width="190" Margin="10"/>
        </StackPanel>
            </MaterialDesign:Card>
```

## 引入图片cartoon.png

### 修改其属性 - 生成操作 为“资源”

### MainWindow.xaml中插入图片

```xaml
                    <Border Margin="10" CornerRadius="20" Background="#FFFFEEFA">
                        <Image Source="/Images/cartoon.png" Stretch="Uniform" Height="150" />
                    </Border>
```

### 修改输入框

#### StaticResource MaterialDesignOutlinedTextBox

#### MaterialDesign:HintAssist.Hint="xxx"

```xaml\
<MaterialDesign:Card Grid.Row="1" Grid.Column="1"
                                 Width="250" Height="440" Margin="10">
        <StackPanel >
                    <Border Margin="10" CornerRadius="20" Background="#FFFFEEFA">
                        <Image Source="/Images/cartoon.png" Stretch="Uniform" Height="150" />
                    </Border>
                    <TextBox 
                        Name="NameTextBox" 
                        Margin="10" 
                        Style="{StaticResource MaterialDesignOutlinedTextBox}"
                        MaterialDesign:HintAssist.Hint="姓名"
                        Text="{Binding SelectedCustomer.Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
                    <TextBox 
                        Name="IdNumberTextBox" 
                        Margin="10"
                        Style="{StaticResource MaterialDesignOutlinedTextBox}"
                        MaterialDesign:HintAssist.Hint="身份证"
                        Text="{Binding SelectedCustomer.IdNumber, Mode=TwoWay}" />
                    <TextBox 
                        Name="AddressTextBox" 
                        Margin="10" 
                        Style="{StaticResource MaterialDesignOutlinedTextBox}"
                        MaterialDesign:HintAssist.Hint="地址"
                        Text="{Binding SelectedCustomer.Address, Mode=TwoWay}" />
                    <Button Content="保存" Margin="10 10 10 30" VerticalAlignment="Bottom" HorizontalAlignment="Left" Click="SaveCustomer_Click" />
                    <Button Content="保存" Margin="10 10 10 30" VerticalAlignment="Bottom" HorizontalAlignment="Left" Click="SaveCustomer_Click" />
        </StackPanel>
            </MaterialDesign:Card>
```

## 测试

![image-20230202111004645](http://qny.expressisland.cn/schoolOpens/image-20230202111004645.png)

# 自定义依赖属性 （预约日历）

## 新建ArrachedProperties文件夹

### CalendarAttachedProperties.cs

```c#
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF_CMS.ArrachedProperties
{
    // Adds a collection of command bindings to a date picker's existing BlackoutDates collection, since the collections are immutable and can't be bound to otherwise.
    // Usage: <DatePicker hacks:AttachedProperties.RegisterBlackoutDates="{Binding BlackoutDates}" >
    public class CalendarAttachedProperties : DependencyObject
    {
        #region Attributes

        private static readonly List<Calendar> _calendars = new List<Calendar>();
        private static readonly List<DatePicker> _datePickers = new List<DatePicker>();

        #endregion

        #region Dependency Properties

        public static DependencyProperty RegisterBlackoutDatesProperty = DependencyProperty.RegisterAttached("RegisterBlackoutDates", typeof(ObservableCollection<DateTime>), typeof(CalendarAttachedProperties), new PropertyMetadata(null, OnRegisterCommandBindingChanged));

        public static void SetRegisterBlackoutDates(DependencyObject d, ObservableCollection<DateTime> value)
        {
            d.SetValue(RegisterBlackoutDatesProperty, value);
        }

        public static ObservableCollection<DateTime> GetRegisterBlackoutDates(DependencyObject d)
        {
            return (ObservableCollection<DateTime>)d.GetValue(RegisterBlackoutDatesProperty);
        }

        #endregion

        #region Event Handlers

        private static void CalendarBindings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<DateTime> blackoutDates = sender as ObservableCollection<DateTime>;

            Calendar calendar = _calendars.First(c => c.Tag == blackoutDates);
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                calendar.BlackoutDates.Clear();
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DateTime date in e.NewItems)
                {
                    calendar.BlackoutDates.Add(new CalendarDateRange(date));
                }
            }
        }

        private static void DatePickerBindings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<DateTime> blackoutDates = sender as ObservableCollection<DateTime>;

            DatePicker datePicker = _datePickers.First(c => c.Tag == blackoutDates);

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DateTime date in e.NewItems)
                {
                    datePicker.BlackoutDates.Add(new CalendarDateRange(date));
                }
            }
        }

        #endregion

        #region Private Methods

        private static void OnRegisterCommandBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Calendar calendar = sender as Calendar;
            if (calendar != null)
            {
                ObservableCollection<DateTime> bindings = e.NewValue as ObservableCollection<DateTime>;
                if (bindings != null)
                {
                    if (!_calendars.Contains(calendar))
                    {
                        calendar.Tag = bindings;
                        _calendars.Add(calendar);
                    }

                    calendar.BlackoutDates.Clear();
                    foreach (DateTime date in bindings)
                    {
                        calendar.BlackoutDates.Add(new CalendarDateRange(date));
                    }
                    bindings.CollectionChanged += CalendarBindings_CollectionChanged;
                }
            }
            else
            {
                DatePicker datePicker = sender as DatePicker;
                if (datePicker != null)
                {
                    ObservableCollection<DateTime> bindings = e.NewValue as ObservableCollection<DateTime>;
                    if (bindings != null)
                    {
                        if (!_datePickers.Contains(datePicker))
                        {
                            datePicker.Tag = bindings;
                            _datePickers.Add(datePicker);
                        }

                        datePicker.BlackoutDates.Clear();
                        foreach (DateTime date in bindings)
                        {
                            datePicker.BlackoutDates.Add(new CalendarDateRange(date));
                        }
                        bindings.CollectionChanged += DatePickerBindings_CollectionChanged;
                    }
                }
            }
        }

        #endregion
    }
}
```

## MainViewModel.cs

### 修改`public ObservableCollection<AppointmentViewModel> Appointments { get; set; } = new();`为`DateTime`

```c#
        /*public ObservableCollection<AppointmentViewModel> Appointments { get; set; } = new();*/
        public ObservableCollection<DateTime> Appointments { get; set; } = new();
```

### 在LoadAppointments方法中对数据进行塑形

### Appointments.Add(a.Time);

```c#
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
```

## MainWindow.xaml修改预约日历

### 引入命名空间

```xaml
        xmlns:li="clr-namespace:WPF_CMS.ArrachedProperties" xmlns:viewmodels="clr-namespace:WPF_CMS.ViewModels" d:DataContext="{d:DesignInstance Type=viewmodels:MainViewModel}"
        mc:Ignorable="d"
```

### `<Calendar>`

* 如果报错则重新生成解决方案。

```xaml
            <MaterialDesign:Card Grid.Row="1" Grid.Column="2" Width="310" Margin="35, 30 35, 30">
            <StackPanel Grid.Row="1" Grid.Column="2">
            <!--<ListView ItemsSource="{Binding Appointments, Mode=TwoWay}" DisplayMemberPath="Time"/>-->
                    <Calendar Name="AppointmentCalender" Height="320" Width="300" li:CalendarAttachedProperties.RegisterBlackoutDates="{Binding Appointments, Mode=OneWay}" SelectedDate="{Binding SelectedDate, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}">
                    </Calendar>
                    <Button Content="预约" Click="AddAppointment_Click"  Width="190" Margin="10"/>
        </StackPanel>
            </MaterialDesign:Card>
```

## 添加日期类型数据（MainViewModel.cs）

```c#
        private DateTime _selectedDate;
        public DateTime SelectedDate { get =>_selectedDate;
            set
            {
                if (_selectedDate != value)
                { 
                    _selectedDate= value;
                    RaisePropertyChange(nameof(SelectedDate));
                }
            }
        }
```

## 测试

* 成功预约，显示为灰色。

![image-20230202232133409](http://qny.expressisland.cn/schoolOpens/image-20230202232133409.png)







