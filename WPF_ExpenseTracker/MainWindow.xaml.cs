using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WPF_ExpenseTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string textFile = "expenses.txt";
        private List<string> expenses = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            dpkDate.SelectedDate = DateTime.Today;
            LoadFromFile();
            
        }//end MainWindow

        private void LoadFromFile()
        {
            try
            {
                //Read each line and add to the list
                using (StreamReader reader = new StreamReader(textFile))
                {
                    string line;
                    //loop through each line
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Add each line to the list
                        expenses.Add(line);
                    }
                    UpdateListBox();
                   

                }

            }
            catch (IOException ex)
            {
                MessageBox.Show($"File Read error: {ex.Message} ");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message} ");
            }



        }//end LoadFromFile

        //bubble sort
        private void BubbleSortExpenses()
        {

            int n = expenses.Count();
            for(int i = 0; i < n - 1; i++)
            {
                for(int j = 0; i < n - i - 1; j++)
                {
                    string[] expense1 = expenses[j].Split("|");
                    string[] expense2 = expenses[j + 1].Split("|");

                    if(expense1.Length > 1 && expense2.Length > 1)
                    {
                        string desc1 = expense1[1].Trim();
                        string desc2 = expense2[1].Trim();

                        if(string.Compare(desc1, desc2, StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            string temp = expenses[j];
                            expenses[j] = expenses[j + 1];
                            expenses[j + 1] = temp;

                        }

                    }

                }//inner loop
            } //end out loop
        }

        //delete expense
        private void BtnDeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button && button.Tag is string selectedExpense)
            {
                expenses.Remove(selectedExpense);

                WriteToFile();
                UpdateListBox();
                

            }
        }

        //update to expenses
        private void UpdateTotalExpenses()
        {
            float total = 0;

            foreach(string expense in expenses)
            {
                string[] parts = expense.Split("|");
                if(parts.Length == 3)
                {
                    string amountText = parts[2].Trim();
                    if(float.TryParse(amountText, NumberStyles.Float, CultureInfo.InvariantCulture, out float amount))
                    {
                        total += amount;
                    }
                }
            } // end foreach

            tbkTotalAmt.Text = total.ToString("F2");
        }

        //write to file
        private void WriteToFile()
        {
            try
            {
                //create streamWriter
                using(StreamWriter writer = new StreamWriter(textFile))
                {
                    foreach (string expense in expenses)
                    {
                        writer.WriteLine(expense);
                    }
                }
            } catch (IOException ex)
            {
                MessageBox.Show($"File Read Error: {ex.Message}");
            } catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }



        



       

      

        private void BtnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            // validate input
            if (dpkDate.SelectedDate == null || string.IsNullOrWhiteSpace(txtDescr.Text) || string.IsNullOrWhiteSpace(txtAmt.Text))
            {
                MessageBox.Show("Please fill all fields correctly.", "Input Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                
            }

            if(!float.TryParse(txtAmt.Text, out float amount))
            {
                MessageBox.Show("Invalid number format. Please enter a valid number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // format the expense "date|descripton|amount"
            string newExpense = $"{dpkDate.SelectedDate:dd-MM-yyyy}|{txtDescr.Text}|{amount:F2}";
            // add expense to list
            expenses.Add(newExpense);
            WriteToFile();      
            //refresh the listbox
            UpdateListBox();
            
            //Clear inputs
            txtDescr.Clear();
            txtAmt.Clear();
            dpkDate.SelectedDate = DateTime.Today;
        }//end BtnAddExpense_Click

        private void UpdateListBox()
        {
            lbxExpenses.ItemsSource = null;
            BubbleSortExpenses();
            lbxExpenses.ItemsSource = expenses;

            //update total expense
            UpdateTotalExpenses();
        }
    }//end class
}//end namespace