using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ProjectDataAnalysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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


                ArrayList[] data = new ArrayList[dialog.FileNames.Length];
                int i = 0;
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
                            while ((line = reader.ReadLine()) != null)
                            {

                                if (startReading)
                                {
                                    string[] words = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries); //rozdelenie slov v riadku
                                    if (double.TryParse(words[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double result)) //skusam slovo dat na double
                                    {
                                        data[i].Add(result); //nacitavam iba prve                                       
                                    }
                                }

                                if (startReading == false && line == "#DATA") //zacnem nacitavat data po tomto slove
                                    startReading = true;

                            }

                        }
                        Console.WriteLine("");


                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that may occur during the file reading process
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }

                    Console.WriteLine("");
                    i++;

                }
            }
        }
    }
}