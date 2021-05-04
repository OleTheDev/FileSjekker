using System;
using System.IO;
using System.Windows.Forms;

namespace FileMatcher
{
    public partial class Form1 : Form
    {
        /*
         * Global vars
         */
        string appPath = AppDomain.CurrentDomain.BaseDirectory;
        FileStream stream = null;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();
        }

        /*
         * Så vi kan flytte rundt må vinduet våres
         */
        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
             * Sjekk om hoved og ny txt filene er laget, vis ikke lager vi de
             */
            try
            {
                if (!File.Exists(appPath + "\\hoved.txt"))
                {
                    stream = File.Create(appPath + "\\hoved.txt");
                    stream.Close();
                }

                if (!File.Exists(appPath + "\\ny.txt"))
                {
                    stream = File.Create(appPath + "\\ny.txt");
                    stream.Close();
                }
            } catch (Exception er)
            {
                MessageBox.Show("Error! \n" + er, "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally
            {
                //Lukke stream
                if (stream != null)
                {
                    stream.Close();
                }
            }
            
            //Last inn filene i listbox
            lastFiler();
        }

        public void lastFiler()
        {
            /*
             * last in alt av informasjon om filene
             */
            if (File.Exists(appPath + "\\hoved.txt"))
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();

                int hovedAmount = 0;
                int nyAmount = 0;
                int missingAmount = 0;

                /*
                 * Hent filer fra hoved mappen 
                 */
                foreach (string hovedLine in File.ReadLines(appPath + "\\hoved.txt"))
                {
                    listBox1.Items.Add(hovedLine);
                    hovedAmount++;
                }

                hovedFiler.Text = hovedAmount.ToString();

                /*
                 * Hent filer fra den nye mappen
                 */
                foreach (string nyLine in File.ReadLines(appPath + "\\ny.txt"))
                {
                    listBox2.Items.Add(nyLine);
                    nyAmount++;
                }

                nyFiler.Text = nyAmount.ToString();

                /*
                 * Sjekk hvilken filer som ikke matcher mellom hoved og ny
                 */
                foreach (string hoved in File.ReadLines(appPath + "\\hoved.txt"))
                {
                    if (!File.ReadAllText(appPath + "\\ny.txt").Contains(hoved))
                    {
                        listBox3.Items.Add(hoved);
                        missingAmount++;
                    }
                }

                filerMangler.Text = missingAmount.ToString();

            }
        }

        /*
         * Kjør sjekk
         */
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                /*
                 * Lag txt filene vis det trengs
                 */
                if (!File.Exists(appPath + "\\hoved.txt"))
                {
                    File.Create(appPath + "\\hoved.txt");
                }

                if (!File.Exists(appPath + "\\ny.txt"))
                {
                    File.Create(appPath + "\\ny.txt");
                }

                //Gjør hoved.txt empty så vi ikke for gammel data
                File.WriteAllText(appPath + "\\hoved.txt", "");

                /*
                 * Skriv inn i hoved txt filen
                 */
                if (textBox1.Text != "")
                {
                    foreach (string hovedFil in Directory.EnumerateFiles(textBox1.Text, "*.*", SearchOption.AllDirectories))
                    {
                        File.AppendAllText(appPath + "\\hoved.txt", "\n" + hovedFil);
                    }
                }

                /*
                 * Skriv inn i ny txt filen
                 */
                if (textBox2.Text != "")
                {
                    foreach (string nyFil in Directory.EnumerateFiles(textBox2.Text, "*.*", SearchOption.AllDirectories))
                    {
                        File.AppendAllText(appPath + "\\ny.txt", "\n" + nyFil);
                    }
                }

                lastFiler();

                MessageBox.Show("Jobb gjennomført!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception er)
            {
                MessageBox.Show("Feil ved fil sjekk! \nPrøv igjen! \n" + er, "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*
         * Exit knappen
         */
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
