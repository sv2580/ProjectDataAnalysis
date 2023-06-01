using System.Collections;
using System.Data;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ProjectDataAnalysis
{
    public partial class Form1 : Form
    {

        private Dictionary<string, double> factors = new Dictionary<string, double>();

        public Form1()
        {
            InitializeComponent();

            //inicializacia dictionary factorov
            factors["0"] = 1;
            factors["2"] = 1.4;
            factors["8"] = 2.2;
            factors["32"] = 3.6;
            factors["128"] = 5;
            factors["512"] = 1;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files | *.sp"; // file types, that will be allowed to upload
            dialog.Multiselect = true; // allow/deny user to upload more than one file at a time
            if (dialog.ShowDialog() == DialogResult.OK) // if user clicked OK
            {
                //pole arraylistov alebo listov, kde kazdy list ma nejaky pocet dat
                //a berie sa vzdy prvy stlpec ktory sa tam ulozi,
                //najprv sa ulozi koncovka teda cislo 0,2 ... ostatne veci mi asi zatial netreba ukladat


                ArrayList[] data = new ArrayList[dialog.FileNames.Length + 1];
                data[0] = new ArrayList(); //arraylist pre tie hodnoty na x linke
                data[0].Add("x");
                int i = 1;
                foreach (String path in dialog.FileNames)
                {
                    data[i] = new ArrayList(); //konkretny array pre data prveho suboru
                    string pattern = @"(?<=m)\d+(?=\.)"; //cisla co su po m a pred . 
                    Match typeOfData = Regex.Match(Path.GetFileName(path), pattern); //chcem extrahovat cislo suboru ako su 0,2 atd pred .sp
                    data[i].Add(typeOfData.Value); //dam ho na prve miesto 
                    try
                    {
                        using (StreamReader reader = new StreamReader(path))
                        {
                            string line;
                            bool startReading = false;
                            int row = 0;
                            while ((line = reader.ReadLine()) != null)
                            {

                                if (startReading)
                                {
                                    string[] words = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries); //rozdelenie slov v riadku
                                    if (i == 1 && double.TryParse(words[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double x))
                                    {
                                        data[0].Add(x); //nacitavam iba druhe
                                    }

                                    if (double.TryParse(words[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double result)) //skusam slovo dat na double
                                    {
                                        data[i].Add(result); //nacitavam iba druhe
                                                             //pri prvom nacitavani aj ten prvy riadok TODO
                                                             //  dataGridView1.Rows[row].Cells[0].Value = result;

                                        row++;
                                    }
                                }

                                if (startReading == false && line == "#DATA") //zacnem nacitavat data po tomto slove
                                    startReading = true;

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that may occur during the file reading process
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                    i++;
                }

                List<double> max = new List<double>();
                double[] maximum = new double[601];
                double[] example = new double[601];



                for (int x = 1; x < data[0].Count; x++)
                {
                    example[x - 1] = (double)data[0][x];
                }


                dataGridView1.DataSource = createBasicGrid(data);
                dataGridView2.DataSource = multiplyDataWithFactors(data, max);


                for (int x = 0; x < max.Count; x++)
                {
                    maximum[x] = max[x];
                }

                formsPlot1.Plot.PlotScatter(maximum,example);

                Console.WriteLine("");

            }
        }

        private DataTable createBasicGrid(ArrayList[] data)
        {
            DataTable dataTable = new DataTable();

            foreach (var column in data)
            {
                dataTable.Columns.Add(column[0].ToString());

            }

            DataRow emptyRow = dataTable.NewRow();
            foreach (var column in data)
            {
                emptyRow[column[0].ToString()] = "-";
            }
            dataTable.Rows.Add(emptyRow);



            for (int j = 1; j < data[0].Count; j++)
            {
                DataRow newRow = dataTable.NewRow();

                foreach (var column in data)
                {
                    newRow[column[0].ToString()] = column[j];
                }

                dataTable.Rows.Add(newRow);
            }

            return dataTable;

        }

        private DataTable multiplyDataWithFactors(ArrayList[] data, List<double> max)
        {

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Max");
            DataRow emptyRow = dataTable.NewRow();
            for (int i = 1; i < data.Length; i++)
            {
                string name = data[i][0].ToString();

                dataTable.Columns.Add(name);
                emptyRow[name] = factors[name];
            }
            emptyRow["Max"] = "-";
            dataTable.Rows.Add(emptyRow);


            for (int i = 1; i < data[0].Count; i++)
            {
                DataRow newRow = dataTable.NewRow();
                double maxValue = double.MinValue;

                for (int j = 1; j < data.Length; j++)
                {
                    double factor = factors[data[j][0].ToString()];
                    double newValue = (double)data[j][i] * factor;
                    newRow[data[j][0].ToString()] = newValue;
                    if (newValue > maxValue)
                        maxValue = newValue;
                }
                newRow["Max"] = maxValue;
                max.Add(maxValue);
                dataTable.Rows.Add(newRow);
            }

            return dataTable;

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void formsPlot1_Load(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}