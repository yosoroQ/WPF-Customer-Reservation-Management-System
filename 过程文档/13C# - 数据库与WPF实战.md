# C# - 数据库与WPF实战

# SQLServer连接

## visual studio连接到数据库

![image-20230131112324168](http://qny.expressisland.cn/schoolOpens/image-20230131112324168.png)

## 添加连接

![image-20230131112215680](http://qny.expressisland.cn/schoolOpens/image-20230131112215680.png)

![image-20230131112418943](http://qny.expressisland.cn/schoolOpens/image-20230131112418943.png)

# data set与table设置

## 新建表（table）

![image-20230131113254859](http://qny.expressisland.cn/schoolOpens/image-20230131113254859.png)

## 表设计页面

![image-20230131113318875](http://qny.expressisland.cn/schoolOpens/image-20230131113318875.png)

## 修改主键列属性

### 标识规范设为true

![image-20230131113440883](http://qny.expressisland.cn/schoolOpens/image-20230131113440883.png)

## 数据结构

```sql
CREATE TABLE [dbo].Customers
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NCHAR(50) NOT NULL, 
    [IdNumber] NCHAR(50) NOT NULL, 
    [Address] NCHAR(100) NOT NULL
)
```

![image-20230131113518866](http://qny.expressisland.cn/schoolOpens/image-20230131113518866.png)

## 更新

### 点击表左上角“更新”

![image-20230131113632967](http://qny.expressisland.cn/schoolOpens/image-20230131113632967.png)

## 刷新表

![image-20230131113724562](http://qny.expressisland.cn/schoolOpens/image-20230131113724562.png)

## 添加数据

![image-20230131114039153](http://qny.expressisland.cn/schoolOpens/image-20230131114039153.png)

# 显示列表型数据：客户列表

## 添加依赖

### System.Data.SqlClient

* ![image-20230131133325805](http://qny.expressisland.cn/schoolOpens/image-20230131133325805.png)

## 实例

### MainWindow.xaml.cs

```c#
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

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
                    ("select * from Customers", _sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable customerTable = new DataTable();
                    sqlDataAdapter.Fill(customerTable);

                    customerList.DisplayMemberPath = "Name";
                    customerList.SelectedValuePath = "Id";
                    customerList.ItemsSource =
                        customerTable.DefaultView;
                }
            }
            catch(Exception e) { 
                MessageBox.Show(e.ToString());
            }
        }
    }
}
```

### MainWindow.xaml

```c#
<Window x:Class="_05SQL.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:_05SQL"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <Label Content="客户列表" HorizontalAlignment="Left" Margin="32,22,0,0" VerticalAlignment="Top"/>
        <ListBox Name="customerList" HorizontalAlignment="Left" Height="229" Margin="32,61,0,0" VerticalAlignment="Top" Width="249"/>
    </Grid>
</Window>
```

## 运行 —— 显示客户列表

![image-20230131135048253](http://qny.expressisland.cn/schoolOpens/image-20230131135048253.png)

# 数据关系与关联表

## 新增表Appointments

```sql
CREATE TABLE [dbo].Appointments
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Time] DATETIME NOT NULL, 
    [CustomerId] INT NOT NULL, 
    CONSTRAINT [FK_Appointments_Customers] FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
)
```

### 表结构设计

![image-20230131140831448](http://qny.expressisland.cn/schoolOpens/image-20230131140831448.png)

## 添加数据

![image-20230131141057356](http://qny.expressisland.cn/schoolOpens/image-20230131141057356.png)

## 测试查询联表数据

```sql
select * from Appointments
Join Customers on Appointments.CustomerId = Customers.Id
where Customers.Id = 1;
```

![image-20230131141357586](http://qny.expressisland.cn/schoolOpens/image-20230131141357586.png)

# 显示关联型数据：客户预约记录

## 新增控件 —— 预约记录

```c#
<Window x:Class="_05SQL.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:_05SQL"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <Label Content="客户列表" HorizontalAlignment="Left" Margin="32,22,0,0" VerticalAlignment="Top"/>
        <ListBox Name="customerList" HorizontalAlignment="Left" Height="229" Margin="32,61,0,0" VerticalAlignment="Top" Width="249"/>

        <!--预约记录-->
        <Label Content="预约记录" HorizontalAlignment="Left" Margin="444,22,0,0" VerticalAlignment="Top"/>
        <ListBox Name="appointmentList" HorizontalAlignment="Left" Height="229" Margin="444,61,0,0" VerticalAlignment="Top" Width="249"/>
    </Grid>
</Window>
```

## customerList_SelectionChanged

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

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
                    ("select * from Customers", _sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable customerTable = new DataTable();
                    sqlDataAdapter.Fill(customerTable);

                    customerList.DisplayMemberPath = "Name";
                    customerList.SelectedValuePath = "Id";
                    customerList.ItemsSource =
                        customerTable.DefaultView;
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
                string query = 
                    "select * from Appointments\r\nJoin Customers on Appointments.CustomerId = Customers.Id\r\nwhere Customers.Id = @CustomerId";

                var customerId = customerList.SelectedValue;

                SqlCommand sqlCommand = new SqlCommand
                    (query,_sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
                    (sqlCommand);

                sqlCommand.Parameters.AddWithValue("@CustomerId",customerId);

                using (sqlDataAdapter)
                {
                    DataTable appointmentTable = new DataTable();
                    sqlDataAdapter.Fill(appointmentTable);

                    appointmentList.DisplayMemberPath = "Time";
                    appointmentList.SelectedValuePath = "Id";
                    appointmentList.ItemsSource =
                        appointmentTable.DefaultView;
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

![image-20230131143130579](http://qny.expressisland.cn/schoolOpens/image-20230131143130579.png)

# 删除数据

## 添加button组件

```c#
<Window x:Class="_05SQL.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:_05SQL"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <Label Content="客户列表" HorizontalAlignment="Left" Margin="32,22,0,0" VerticalAlignment="Top"/>
        <ListBox Name="customerList" HorizontalAlignment="Left" Height="229" Margin="32,61,0,0" VerticalAlignment="Top" Width="249" SelectionChanged="customerList_SelectionChanged"/>

        <!--预约记录-->
        <Label Content="预约记录" HorizontalAlignment="Left" Margin="444,22,0,0" VerticalAlignment="Top"/>
        <ListBox Name="appointmentList" HorizontalAlignment="Left" Height="229" Margin="444,61,0,0" VerticalAlignment="Top" Width="249"/>
        
        <!--button-->
        <Button Content="删除客户" HorizontalAlignment="Left" Margin="32,306,0,0" VerticalAlignment="Top" Width="249" Click="DeleteCustomer_Click"/>
        <Button Content="取消预约" HorizontalAlignment="Left" Margin="444,306,0,0" VerticalAlignment="Top" Width="249" Click="DeleteAppointment_Click"/>
    </Grid>
</Window>
```

## 取消预约

### DeleteAppointment_Click

### DeleteCustomer_Click

```c#
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

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
                    ("select * from Customers", _sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable customerTable = new DataTable();
                    sqlDataAdapter.Fill(customerTable);

                    customerList.DisplayMemberPath = "Name";
                    customerList.SelectedValuePath = "Id";
                    customerList.ItemsSource =
                        customerTable.DefaultView;
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
                string query = 
                    "select * from Appointments\r\nJoin Customers on Appointments.CustomerId = Customers.Id\r\nwhere Customers.Id = @CustomerId";

                var customerId = customerList.SelectedValue;

                if (customerId == null) {
                    appointmentList.ItemsSource = null;
                    return;
                }

                SqlCommand sqlCommand = new SqlCommand
                    (query,_sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
                    (sqlCommand);

                sqlCommand.Parameters.AddWithValue("@CustomerId",customerId);

                using (sqlDataAdapter)
                {
                    DataTable appointmentTable = new DataTable();
                    sqlDataAdapter.Fill(appointmentTable);

                    appointmentList.DisplayMemberPath = "Time";
                    appointmentList.SelectedValuePath = "Id";
                    appointmentList.ItemsSource =
                        appointmentTable.DefaultView;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //取消预约
        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sql = "delete from Appointments where Id = @AppointmentId";
                var appointmentId = appointmentList.SelectedValue;

                SqlCommand sqlCommand = new SqlCommand
                    (sql, _sqlConnection);
                sqlCommand.Parameters.AddWithValue
                    ("@AppointmentId", appointmentId);


                _sqlConnection.Open();
                sqlCommand.ExecuteScalar();

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally {
                _sqlConnection.Close();
                customerList_SelectionChanged(null, null);
            }
}

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            try {
                string sqlDeleteAppointment =
                    "delete from Appointments  where CustomerId =@CustomerId;";
                string sqlDeleteCustomer =
    "delete from Customers where id =@CustomerId;";

                var customerId = customerList.SelectedValue;

                SqlCommand cmd1 = new SqlCommand
                    (sqlDeleteAppointment,_sqlConnection);
                SqlCommand cmd2 = new SqlCommand
    (sqlDeleteCustomer, _sqlConnection);

                cmd1.Parameters.AddWithValue("@CustomerId",customerId);
                cmd2.Parameters.AddWithValue("@CustomerId", customerId);

                _sqlConnection.Open();
                cmd1.ExecuteScalar();
                cmd2.ExecuteScalar();

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                _sqlConnection.Close();
                showCustomers();
                customerList_SelectionChanged(null, null);
            }
        }
    }
}
```

# 添加数据

## 新增数据相关控件

```xaml
<Window x:Class="_05SQL.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:_05SQL"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <Label Content="客户列表" HorizontalAlignment="Left" Margin="32,22,0,0" VerticalAlignment="Top"/>
        <ListBox Name="customerList" HorizontalAlignment="Left" Height="229" Margin="32,61,0,0" VerticalAlignment="Top" Width="249" SelectionChanged="customerList_SelectionChanged"/>

        <!--预约记录-->
        <Label Content="预约记录" HorizontalAlignment="Left" Margin="444,22,0,0" VerticalAlignment="Top"/>
        <ListBox Name="appointmentList" HorizontalAlignment="Left" Height="229" Margin="444,61,0,0" VerticalAlignment="Top" Width="249"/>
        
        <!--button-->
        <Button Content="删除客户" HorizontalAlignment="Left" Margin="32,306,0,0" VerticalAlignment="Top" Width="249" Click="DeleteCustomer_Click"/>
        <Button Content="取消预约" HorizontalAlignment="Left" Margin="444,306,0,0" VerticalAlignment="Top" Width="249" Click="DeleteAppointment_Click"/>

        <!--新增数据-->
        <TextBox Name="NameTextBox" HorizontalAlignment="Left" Margin="32,359,0,0" Text="TextBox" TextWrapping="Wrap" VerticalAlignment="Top" Width="120"/>
        <TextBox Name="IdTextBox" HorizontalAlignment="Left" Margin="322,359,0,0" Text="TextBox" TextWrapping="Wrap" VerticalAlignment="Top" Width="120"/>
        <TextBox Name="AddressTextBox" HorizontalAlignment="Left" Margin="175,359,0,0" Text="TextBox" TextWrapping="Wrap" VerticalAlignment="Top" Width="120"/>
        <Label Content="姓名" HorizontalAlignment="Left" Margin="32,331,0,0" VerticalAlignment="Top"/>
        <Label Content="身份证" HorizontalAlignment="Left" Margin="175,333,0,0" VerticalAlignment="Top"/>
        <Label Content="住址" HorizontalAlignment="Left" Margin="322,331,0,0" VerticalAlignment="Top"/>
        <Button Content="添加客户" HorizontalAlignment="Left" Margin="32,382,0,0" VerticalAlignment="Top" Click="AddCustomer_Click"/>
        <DatePicker Name="AppointmentDatePicker" HorizontalAlignment="Left" Margin="467,356,0,0" VerticalAlignment="Top"/>
        <Button Content="预约" HorizontalAlignment="Left" Margin="589,359,0,0" VerticalAlignment="Top" Click="AddAppointment_Click"/>
    </Grid>
</Window>
```

## 添加客户 和 预约

### AddCustomer_Click

### AddAppointment_Click

```c#
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

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
                    ("select * from Customers", _sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable customerTable = new DataTable();
                    sqlDataAdapter.Fill(customerTable);

                    customerList.DisplayMemberPath = "Name";
                    customerList.SelectedValuePath = "Id";
                    customerList.ItemsSource =
                        customerTable.DefaultView;
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
                string query = 
                    "select * from Appointments\r\nJoin Customers on Appointments.CustomerId = Customers.Id\r\nwhere Customers.Id = @CustomerId";

                var customerId = customerList.SelectedValue;

                if (customerId == null) {
                    appointmentList.ItemsSource = null;
                    return;
                }

                SqlCommand sqlCommand = new SqlCommand
                    (query,_sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
                    (sqlCommand);

                sqlCommand.Parameters.AddWithValue("@CustomerId",customerId);

                using (sqlDataAdapter)
                {
                    DataTable appointmentTable = new DataTable();
                    sqlDataAdapter.Fill(appointmentTable);

                    appointmentList.DisplayMemberPath = "Time";
                    appointmentList.SelectedValuePath = "Id";
                    appointmentList.ItemsSource =
                        appointmentTable.DefaultView;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //取消预约
        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sql = "delete from Appointments where Id = @AppointmentId";
                var appointmentId = appointmentList.SelectedValue;

                SqlCommand sqlCommand = new SqlCommand
                    (sql, _sqlConnection);
                sqlCommand.Parameters.AddWithValue
                    ("@AppointmentId", appointmentId);


                _sqlConnection.Open();
                sqlCommand.ExecuteScalar();

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally {
                _sqlConnection.Close();
                customerList_SelectionChanged(null, null);
            }
}

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            try {
                string sqlDeleteAppointment =
                    "delete from Appointments  where CustomerId =@CustomerId;";
                string sqlDeleteCustomer =
    "delete from Customers where id =@CustomerId;";

                var customerId = customerList.SelectedValue;

                SqlCommand cmd1 = new SqlCommand
                    (sqlDeleteAppointment,_sqlConnection);
                SqlCommand cmd2 = new SqlCommand
    (sqlDeleteCustomer, _sqlConnection);

                cmd1.Parameters.AddWithValue("@CustomerId",customerId);
                cmd2.Parameters.AddWithValue("@CustomerId", customerId);

                _sqlConnection.Open();
                cmd1.ExecuteScalar();
                cmd2.ExecuteScalar();

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                _sqlConnection.Close();
                showCustomers();
                customerList_SelectionChanged(null, null);
            }
        }

        //添加客户
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            try {

                var sql = "insert into Customers values" +
                    "(@name,@id,@address)";
                SqlCommand sqlCommand = new SqlCommand
                    (sql, _sqlConnection);

                sqlCommand.Parameters.AddWithValue
                    ("@name",NameTextBox.Text);
                sqlCommand.Parameters.AddWithValue
    ("@id", IdTextBox.Text);
                sqlCommand.Parameters.AddWithValue
    ("@address", AddressTextBox.Text);


                _sqlConnection.Open();
                sqlCommand.ExecuteScalar();

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                _sqlConnection.Close();
                showCustomers();
            }
        }

        //预约
        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sql = "insert into Appointments values" +
                    "(@date,@customerId)";

                SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
                sqlCommand.Parameters.AddWithValue("@date",AppointmentDatePicker.Text);
                sqlCommand.Parameters.AddWithValue("@customerId",customerList.SelectedValue);

                _sqlConnection.Open();
                sqlCommand.ExecuteScalar();
            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                _sqlConnection.Close();
                customerList_SelectionChanged(null,null);
            }
        }
    }
}
```



## 运行测试

![image-20230131155639352](http://qny.expressisland.cn/schoolOpens/image-20230131155639352.png)

# 更新数据

## 更新客户资料button

```xaml
<Window x:Class="_05SQL.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:_05SQL"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <Label Content="客户列表" HorizontalAlignment="Left" Margin="32,22,0,0" VerticalAlignment="Top"/>
        <ListBox Name="customerList" HorizontalAlignment="Left" Height="229" Margin="32,61,0,0" VerticalAlignment="Top" Width="249" SelectionChanged="customerList_SelectionChanged"/>

        <!--预约记录-->
        <Label Content="预约记录" HorizontalAlignment="Left" Margin="444,22,0,0" VerticalAlignment="Top"/>
        <ListBox Name="appointmentList" HorizontalAlignment="Left" Height="229" Margin="444,61,0,0" VerticalAlignment="Top" Width="249"/>
        
        <!--button-->
        <Button Content="删除客户" HorizontalAlignment="Left" Margin="32,306,0,0" VerticalAlignment="Top" Width="249" Click="DeleteCustomer_Click"/>
        <Button Content="取消预约" HorizontalAlignment="Left" Margin="444,306,0,0" VerticalAlignment="Top" Width="249" Click="DeleteAppointment_Click"/>

        <!--新增数据-->
        <TextBox Name="NameTextBox" HorizontalAlignment="Left" Margin="32,359,0,0" Text="TextBox" TextWrapping="Wrap" VerticalAlignment="Top" Width="120"/>
        <TextBox Name="AddressTextBox" HorizontalAlignment="Left" Margin="322,359,0,0" Text="TextBox" TextWrapping="Wrap" VerticalAlignment="Top" Width="120"/>
        <TextBox Name="IdTextBox" HorizontalAlignment="Left" Margin="175,359,0,0" Text="TextBox" TextWrapping="Wrap" VerticalAlignment="Top" Width="120"/>
        <Label Content="姓名" HorizontalAlignment="Left" Margin="32,331,0,0" VerticalAlignment="Top"/>
        <Label Content="身份证" HorizontalAlignment="Left" Margin="175,333,0,0" VerticalAlignment="Top"/>
        <Label Content="住址" HorizontalAlignment="Left" Margin="322,331,0,0" VerticalAlignment="Top"/>
        <Button Content="添加客户" HorizontalAlignment="Left" Margin="32,382,0,0" VerticalAlignment="Top" Click="AddCustomer_Click"/>
        <DatePicker Name="AppointmentDatePicker" HorizontalAlignment="Left" Margin="467,356,0,0" VerticalAlignment="Top"/>
        <Button Content="预约" HorizontalAlignment="Left" Margin="589,359,0,0" VerticalAlignment="Top" Click="AddAppointment_Click"/>

        <!--更新客户资料-->
        <Button Content="更新客户资料" HorizontalAlignment="Left" Margin="113,382,0,0" VerticalAlignment="Top" Click="UpdateCustomer_Click"/>
    </Grid>
</Window>
```

## 更新客户资料

### UpdateCustomer_Click

```c#
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

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
                    ("select * from Customers", _sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable customerTable = new DataTable();
                    sqlDataAdapter.Fill(customerTable);

                    customerList.DisplayMemberPath = "Name";
                    customerList.SelectedValuePath = "Id";
                    customerList.ItemsSource =
                        customerTable.DefaultView;
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
                string query = 
                    "select * from Appointments\r\nJoin Customers on Appointments.CustomerId = Customers.Id\r\nwhere Customers.Id = @CustomerId";

                var customerId = customerList.SelectedValue;

                if (customerId == null) {
                    appointmentList.ItemsSource = null;
                    return;
                }

                DataRowView selectedItem = customerList.SelectedItem as DataRowView;
                NameTextBox.Text = selectedItem["Name"] as string;
                IdTextBox.Text = selectedItem["IdNumber"] as string;
                AddressTextBox.Text = selectedItem["Address"] as string;

                SqlCommand sqlCommand = new SqlCommand
                    (query,_sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
                    (sqlCommand);

                sqlCommand.Parameters.AddWithValue("@CustomerId",customerId);

                using (sqlDataAdapter)
                {
                    DataTable appointmentTable = new DataTable();
                    sqlDataAdapter.Fill(appointmentTable);

                    appointmentList.DisplayMemberPath = "Time";
                    appointmentList.SelectedValuePath = "Id";
                    appointmentList.ItemsSource =
                        appointmentTable.DefaultView;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //取消预约
        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sql = "delete from Appointments where Id = @AppointmentId";
                var appointmentId = appointmentList.SelectedValue;

                SqlCommand sqlCommand = new SqlCommand
                    (sql, _sqlConnection);
                sqlCommand.Parameters.AddWithValue
                    ("@AppointmentId", appointmentId);


                _sqlConnection.Open();
                sqlCommand.ExecuteScalar();

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally {
                _sqlConnection.Close();
                customerList_SelectionChanged(null, null);
            }
}

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            try {
                string sqlDeleteAppointment =
                    "delete from Appointments  where CustomerId =@CustomerId;";
                string sqlDeleteCustomer =
    "delete from Customers where id =@CustomerId;";

                var customerId = customerList.SelectedValue;

                SqlCommand cmd1 = new SqlCommand
                    (sqlDeleteAppointment,_sqlConnection);
                SqlCommand cmd2 = new SqlCommand
    (sqlDeleteCustomer, _sqlConnection);

                cmd1.Parameters.AddWithValue("@CustomerId",customerId);
                cmd2.Parameters.AddWithValue("@CustomerId", customerId);

                _sqlConnection.Open();
                cmd1.ExecuteScalar();
                cmd2.ExecuteScalar();

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                _sqlConnection.Close();
                showCustomers();
                customerList_SelectionChanged(null, null);
            }
        }

        //添加客户
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            try {

                var sql = "insert into Customers values" +
                    "(@name,@id,@address)";
                SqlCommand sqlCommand = new SqlCommand
                    (sql, _sqlConnection);

                sqlCommand.Parameters.AddWithValue
                    ("@name",NameTextBox.Text);
                sqlCommand.Parameters.AddWithValue
    ("@id", IdTextBox.Text);
                sqlCommand.Parameters.AddWithValue
    ("@address", AddressTextBox.Text);


                _sqlConnection.Open();
                sqlCommand.ExecuteScalar();

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                _sqlConnection.Close();
                showCustomers();
            }
        }

        //预约
        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sql = "insert into Appointments values" +
                    "(@date,@customerId)";

                SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
                sqlCommand.Parameters.AddWithValue("@date",AppointmentDatePicker.Text);
                sqlCommand.Parameters.AddWithValue("@customerId",customerList.SelectedValue);

                _sqlConnection.Open();
                sqlCommand.ExecuteScalar();
            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                _sqlConnection.Close();
                customerList_SelectionChanged(null,null);
            }
        }

        private void UpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            try {
                var sql = "update Customers set Name=@name, IdNumber=@idNumber, Address=@address where Id=@customerId";

                SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
                sqlCommand.Parameters.AddWithValue("@name", NameTextBox.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@idNumber", IdTextBox.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@address", AddressTextBox.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@customerId", customerList.SelectedValue);

                _sqlConnection.Open();
                sqlCommand.ExecuteScalar();

            }
            catch (Exception error)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                _sqlConnection.Close();
                showCustomers();
            }
        }
    }
}
```

## 运行测试

![image-20230131162441267](http://qny.expressisland.cn/schoolOpens/image-20230131162441267.png)







