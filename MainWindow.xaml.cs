using AutoLotModel;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace Neag_Cristina_lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;

        Binding txtFirstNameBinding = new Binding();
        Binding txtLastNameBinding = new Binding();
        Binding txtMakeBinding = new Binding();
        Binding txtColorBinding = new Binding();
        Binding txtCarIdBinding = new Binding();
        Binding txtCustIdBinding = new Binding();
        Binding txtOrderIdBinding = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            txtFirstNameBinding.Path = new PropertyPath("FirstName");
            txtLastNameBinding.Path = new PropertyPath("LastName");
            txtMakeBinding.Path = new PropertyPath("Make");
            txtColorBinding.Path = new PropertyPath("Color");
            txtCarIdBinding.Path = new PropertyPath("CarId");
            txtCarIdBinding.Mode = BindingMode.OneWay;
            txtCustIdBinding.Path = new PropertyPath("CustId");
            txtCustIdBinding.Mode = BindingMode.OneWay;
            txtOrderIdBinding.Path = new PropertyPath("OrderId");
            txtOrderIdBinding.Mode = BindingMode.OneWay;
            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            txtCarId.SetBinding(TextBox.TextProperty, txtCarIdBinding);
            txtCustId.SetBinding(TextBox.TextProperty, txtCustIdBinding);
            txtOrderId.SetBinding(TextBox.TextProperty, txtOrderIdBinding);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

            //using System.Data.Entity;
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Customers.Load();
            System.Windows.Data.CollectionViewSource inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;
            ctx.Inventories.Load();
            ctx.Orders.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";

            BindDataGrid();
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
         
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnNext.IsEnabled = true;
                btnPrev.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else
            if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnNext.IsEnabled = true;
                btnPrev.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                customerViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);
            }
            else
            if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnNext.IsEnabled = true;
                btnPrev.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                customerViewSource.View.Refresh();
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnNext.IsEnabled = false;
            btnPrev.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnNext.IsEnabled = true;
            btnPrev.IsEnabled = true;
            customerDataGrid.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;

            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnNext.IsEnabled = false;
            btnPrev.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            SetValidationBinding();

            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnNext.IsEnabled = false;
            btnPrev.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
        }

        private void btnNew_Inv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNew_Inv.IsEnabled = false;
            btnEdit_Inv.IsEnabled = false;
            btnDelete_Inv.IsEnabled = false;
            btnSave_Inv.IsEnabled = true;
            btnCancel_Inv.IsEnabled = true;
            btnNext_Inv.IsEnabled = false;
            btnPrev_Inv.IsEnabled = false;
            inventoryDataGrid.IsEnabled = false;
            carIdTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = "";
            makeTextBox.Text = "";
            Keyboard.Focus(colorTextBox);
        }

        private void btnEdit_Inv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnNew_Inv.IsEnabled = false;
            btnEdit_Inv.IsEnabled = false;
            btnDelete_Inv.IsEnabled = false;
            btnSave_Inv.IsEnabled = true;
            btnCancel_Inv.IsEnabled = true;
            btnNext_Inv.IsEnabled = false;
            btnPrev_Inv.IsEnabled = false;
            inventoryDataGrid.IsEnabled = false;
            carIdTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
            Keyboard.Focus(colorTextBox);
        }

        private void btnDelete_Inv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnNew_Inv.IsEnabled = false;
            btnEdit_Inv.IsEnabled = false;
            btnDelete_Inv.IsEnabled = false;
            btnSave_Inv.IsEnabled = true;
            btnCancel_Inv.IsEnabled = true;
            btnNext_Inv.IsEnabled = false;
            btnPrev_Inv.IsEnabled = false;
            inventoryDataGrid.IsEnabled = false;
            carIdTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
        }

        private void btnSave_Inv_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    inventory = new Inventory()
                    {
                        Make = makeTextBox.Text.Trim(),
                        Color = colorTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(inventory);
                    //inventoryViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew_Inv.IsEnabled = true;
                btnEdit_Inv.IsEnabled = true;
                btnDelete_Inv.IsEnabled = true;
                btnSave_Inv.IsEnabled = false;
                btnCancel_Inv.IsEnabled = false;
                btnNext_Inv.IsEnabled = true;
                btnPrev_Inv.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                carIdTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }
            else
            if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Make = colorTextBox.Text.Trim();
                    inventory.Color = makeTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew_Inv.IsEnabled = true;
                btnEdit_Inv.IsEnabled = true;
                btnDelete_Inv.IsEnabled = true;
                btnSave_Inv.IsEnabled = false;
                btnCancel_Inv.IsEnabled = false;
                btnNext_Inv.IsEnabled = true;
                btnPrev_Inv.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                carIdTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
                customerViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(inventory);
            }
            else
            if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew_Inv.IsEnabled = true;
                btnEdit_Inv.IsEnabled = true;
                btnDelete_Inv.IsEnabled = true;
                btnSave_Inv.IsEnabled = false;
                btnCancel_Inv.IsEnabled = false;
                btnNext_Inv.IsEnabled = true;
                btnPrev_Inv.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                carIdTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerViewSource.View.Refresh();
            }
        }

        private void btnCancel_Inv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew_Inv.IsEnabled = true;
            btnEdit_Inv.IsEnabled = true;
            btnDelete_Inv.IsEnabled = true;
            btnSave_Inv.IsEnabled = false;
            btnCancel_Inv.IsEnabled = false;
            btnNext_Inv.IsEnabled = true;
            btnPrev_Inv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = true;
            carIdTextBox.IsEnabled = false;
            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;
            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
        }

        private void btnPrev_Inv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Inv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }

        private void btnNew_Ord_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNew_Ord.IsEnabled = false;
            btnEdit_Ord.IsEnabled = false;
            btnDelete_Ord.IsEnabled = false;
            btnSave_Ord.IsEnabled = true;
            btnCancel_Ord.IsEnabled = true;
            btnNext_Ord.IsEnabled = false;
            btnPrev_Ord.IsEnabled = false;
            ordersDataGrid.IsEnabled = false;
            txtCarId.IsEnabled = true;
            txtCustId.IsEnabled = true;
            txtOrderId.IsEnabled = false;

            BindingOperations.ClearBinding(txtCarId, TextBox.TextProperty);
            BindingOperations.ClearBinding(txtCustId, TextBox.TextProperty);
            txtCarId.Text = "";
            txtCustId.Text = "";
            Keyboard.Focus(txtCarId);
        }

        private void btnEdit_Ord_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempCustId = txtCustId.Text.ToString();
            string tempCarId = txtCarId.Text.ToString();

            btnNew_Ord.IsEnabled = false;
            btnEdit_Ord.IsEnabled = false;
            btnDelete_Ord.IsEnabled = false;
            btnSave_Ord.IsEnabled = true;
            btnCancel_Ord.IsEnabled = true;
            btnNext_Ord.IsEnabled = false;
            btnPrev_Ord.IsEnabled = false;
            ordersDataGrid.IsEnabled = false;
            txtCarId.IsEnabled = true;
            txtCustId.IsEnabled = true;
            txtOrderId.IsEnabled = false;

            BindingOperations.ClearBinding(txtCarId, TextBox.TextProperty);
            BindingOperations.ClearBinding(txtCustId, TextBox.TextProperty);
            txtCarId.Text = tempCarId;
            txtCustId.Text = tempCustId;
            Keyboard.Focus(txtCarId);
        }

        private void btnDelete_Ord_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempCustId = txtCustId.Text.ToString();
            string tempCarId = txtCarId.Text.ToString();

            btnNew_Ord.IsEnabled = false;
            btnEdit_Ord.IsEnabled = false;
            btnDelete_Ord.IsEnabled = false;
            btnSave_Ord.IsEnabled = true;
            btnCancel_Ord.IsEnabled = true;
            btnNext_Ord.IsEnabled = false;
            btnPrev_Ord.IsEnabled = false;
            ordersDataGrid.IsEnabled = false;
            txtCarId.IsEnabled = true;
            txtCustId.IsEnabled = true;
            txtOrderId.IsEnabled = false;

            BindingOperations.ClearBinding(txtCarId, TextBox.TextProperty);
            BindingOperations.ClearBinding(txtCustId, TextBox.TextProperty);
            txtCarId.Text = tempCarId;
            txtCustId.Text = tempCustId;
        }

        private void btnSave_Ord_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew_Ord.IsEnabled = true;
                btnEdit_Ord.IsEnabled = true;
                btnDelete_Ord.IsEnabled = true;
                btnSave_Ord.IsEnabled = false;
                btnCancel_Ord.IsEnabled = false;
                btnNext_Ord.IsEnabled = true;
                btnPrev_Ord.IsEnabled = true;
                ordersDataGrid.IsEnabled = true;
                txtCarId.IsEnabled = true;
                txtCustId.IsEnabled = true;
                txtOrderId.IsEnabled = false;
            }
            else
            if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                    editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                btnNew_Ord.IsEnabled = true;
                btnEdit_Ord.IsEnabled = true;
                btnDelete_Ord.IsEnabled = true;
                btnSave_Ord.IsEnabled = false;
                btnCancel_Ord.IsEnabled = false;
                btnNext_Ord.IsEnabled = true;
                btnPrev_Ord.IsEnabled = true;
                ordersDataGrid.IsEnabled = true;
                txtCarId.IsEnabled = true;
                txtCustId.IsEnabled = true;
                txtOrderId.IsEnabled = false;
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(selectedOrder);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew_Ord.IsEnabled = true;
                btnEdit_Ord.IsEnabled = true;
                btnDelete_Ord.IsEnabled = true;
                btnSave_Ord.IsEnabled = false;
                btnCancel_Ord.IsEnabled = false;
                btnNext_Ord.IsEnabled = true;
                btnPrev_Ord.IsEnabled = true;
                ordersDataGrid.IsEnabled = true;
                txtCarId.IsEnabled = true;
                txtCustId.IsEnabled = true;
                txtOrderId.IsEnabled = false;
            }
        }

        private void btnCancel_Ord_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew_Ord.IsEnabled = true;
            btnEdit_Ord.IsEnabled = true;
            btnDelete_Ord.IsEnabled = true;
            btnSave_Ord.IsEnabled = false;
            btnCancel_Ord.IsEnabled = false;
            btnNext_Ord.IsEnabled = true;
            btnPrev_Ord.IsEnabled = true;
            ordersDataGrid.IsEnabled = true;
            txtCarId.IsEnabled = true;
            txtCustId.IsEnabled = true;
            txtOrderId.IsEnabled = false;

            txtCarId.SetBinding(TextBox.TextProperty, txtCarIdBinding);
            txtCustId.SetBinding(TextBox.TextProperty, txtCustIdBinding);
        }

        private void btnPrev_Ord_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Ord_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding); //setare binding nou
        }
    }
}
