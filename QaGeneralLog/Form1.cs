using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Collections;
using System.IO;

namespace QaGeneralLog
{

    public partial class Form1 : Form
    {
       
        ArrayList BugreportAcikCmdList = new ArrayList();
        String KullAdi = SystemInformation.UserName;
        ArrayList BagliSeriNumaralar = new ArrayList();
        ArrayList BaslatilmisSeriNumaralar = new ArrayList();
        ArrayList CmdIdList = new ArrayList();
        DirectoryInfo LogKlasorYol;
        ArrayList AcikCmd = new ArrayList();
        ArrayList TumCmdList = new ArrayList();
        int Saniye, Dakika, Saat, Gun,Zamanlayici;
        String TestID2 = " ";
       
        String KomutAdb, HataDuzeyi;
        ArrayList BugreportCmdList = new ArrayList();
        int LogTekrar = 1;

        public Form1()
        {
            InitializeComponent();
        }
  
        private void button1_Click(object sender, EventArgs e)
        {           
            CihazTespitEt();         
        }

        private void CihazTespitEt() {
          
            string[] SeriNumaraList = AdbKomut("adb devices").Split('\n');
            string Seri = "";
            ArrayList SilinecekSeriNum = new ArrayList();
            ArrayList GuncelEklenenSeriler = new ArrayList();
                    
            for (int i = 1; i < SeriNumaraList.Count()-2; i++)
            {

                Seri = SeriNumaraList[i].Replace("device", "");

                if (Seri.Contains("daemon") == true)
                {
                    CihazTespitEt();
                    return;
                }else if(Seri != "" && BagliSeriNumaralar.Contains(Seri) == false) {

                    listView1.Items.Add(Seri);
                    BagliSeriNumaralar.Add(Seri);    
                    
                }

                GuncelEklenenSeriler.Add(Seri);
            }          

            for (int z = 0; z < BagliSeriNumaralar.Count; z++)
            {
                
                if (GuncelEklenenSeriler.Contains(BagliSeriNumaralar[z].ToString()) == false)
                    {
                   
                    SilinecekSeriNum.Add(BagliSeriNumaralar[z]);

                    }
        
                for (int g = 0; g < SilinecekSeriNum.Count; g++)
                {

                    try {                    
                    BagliSeriNumaralar.Remove(SilinecekSeriNum[g].ToString());

                    int ListCount = listView1.Items.Count;

                    for (int y = 0; y < ListCount; y++)
                    {
                    
                        if (listView1.Items[y].Text == SilinecekSeriNum[g].ToString())
                        {                           
                            listView1.Items[y].Remove();
                        }
                    }

                    }
                    catch (Exception ex)
                    {
                        return;
                    }

                }

            }

            label1.Text = "Bağlı Cihaz Sayısı : " + (SeriNumaraList.Count() - 3).ToString();
        }

        public string AdbKomut(string Komut)
        {
            try
            {
                Process My_Process = new Process();
                ProcessStartInfo My_Process_Info = new ProcessStartInfo();
                My_Process_Info.FileName = "cmd.exe";
                My_Process_Info.Arguments = "/c " + Komut;
                My_Process_Info.WorkingDirectory = "C:\\Users\\" + KullAdi + "\\Documents\\QaGeneral\\xtool_scripts";
                My_Process_Info.CreateNoWindow = true;
                My_Process_Info.UseShellExecute = false;
                My_Process_Info.RedirectStandardOutput = true;
                My_Process_Info.RedirectStandardError = true;
                My_Process.EnableRaisingEvents = true;
                My_Process.StartInfo = My_Process_Info;
                My_Process.Start();
                string Process_StandardOutput = My_Process.StandardOutput.ReadToEnd();
                if (Process_StandardOutput != null)
                {
                    My_Process.Dispose();
                    return Process_StandardOutput;
                }
            }
            catch (Exception ex)
            {
                return "HATA : " + ex.Message;
            }

            return "OK";
        }

        private void button4_Click(object sender, EventArgs e)
        {

            for(int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Checked = true;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Zamanlayici = 0;
            LogTekrar = 1;
            CihazTespitEt();
            radioButton8.Enabled = false;
            radioButton9.Enabled = false;

            if (Directory.Exists(@"" + textBox2.Text) == false)
            {
                Directory.CreateDirectory(@"" + textBox2.Text);
            }

          
            for (int i = 0; i < BagliSeriNumaralar.Count; i++)
            {

                if (Directory.Exists(@"" + textBox2.Text + "\\" + BagliSeriNumaralar[i].ToString()) == false)
                {
                    Directory.CreateDirectory(@"" + textBox2.Text + "\\" + BagliSeriNumaralar[i].ToString());
                }

            }
          
            HataDuzeyi = "";         

            if (radioButton1.Checked) // V Ayrıntılı
            {
                KomutAdb = "*:V";
                HataDuzeyi = "(V)_Ayrıntılı";
            }
            else if (radioButton2.Checked) // D Hata Ayıklama
            {
                KomutAdb = "*:D";
                HataDuzeyi = "(D)_HataAyıklama";
            }
            else if (radioButton3.Checked) // I Bilgi
            {
                KomutAdb = "*:I";
                HataDuzeyi = "(I)_Bilgi";
            }
            else if (radioButton4.Checked) // W Uyarı
            {
                KomutAdb = "*:W";
                HataDuzeyi = "(W)_Uyarı";
            }
            else if (radioButton5.Checked) // E Hata
            {
                KomutAdb = "*:E";
                HataDuzeyi = "(E)_Hata";
            }
            else if (radioButton6.Checked) // F Yuksek Hata
            {
                KomutAdb = "*:F";
                HataDuzeyi = "(F)_YüksekHata";
            }
            else if (radioButton7.Checked) // S En Yuksek Hata
            {
                KomutAdb = "*:S";
                HataDuzeyi = "(V)_EnYuksekHata";
            }
            else
            {
                KomutAdb = "Hata";             
            }

            if(KomutAdb.Contains("Hata") == true){

                MessageBox.Show("Hata Seçenekleri Kontrol Ediniz.");
                return;

            }

            LogAl();
           label16.Text= "Yapılan İşlem : Log Başlatıldı";
        
        }


        public void LogAl() {
            string LogTemizle = "";
            if (checkBox2.Checked)
            {
                LogTemizle = "adb logcat -c & ";
            }
         
            ArrayList BaslatilmisLoglar = new ArrayList();
            CmdIdList.Clear();
            Boolean IslemYapildi = false;
            
            foreach (Process islem in Process.GetProcessesByName("cmd"))
            {
                CmdIdList.Add(islem.Id.ToString());
            }

            for (int i = 0; i < BagliSeriNumaralar.Count; i++) {

                for (int a = 0; a < BaslatilmisSeriNumaralar.Count; a++)
                {
                    if(BaslatilmisSeriNumaralar[a].ToString().Split(',')[0] == BagliSeriNumaralar[i].ToString().Trim().Replace(" ", string.Empty))
                    {
                        BaslatilmisLoglar.Add(BaslatilmisSeriNumaralar[a].ToString().Split(',')[2]);
                    }
                }

                if (checkBox3.Checked && BaslatilmisLoglar.Contains("Bugreport") == false) // Bugreport
                {
                    LogBaslatKomutGonder(i, LogTemizle + "adb bugreport", BagliSeriNumaralar[i].ToString().Trim().Replace(" ", string.Empty), HataDuzeyi.ToString().Trim().Trim().Replace(" ", string.Empty), "Bugreport");
                    IslemYapildi = true;
                }
                                                
                if (checkBox4.Checked && BaslatilmisLoglar.Contains("Logcat_Main") == false) //Adb Logcat
                {
                    LogBaslatKomutGonder(i, LogTemizle + "adb logcat " + KomutAdb, BagliSeriNumaralar[i].ToString().Trim().Replace(" ", string.Empty), HataDuzeyi.ToString().Trim().Replace(" ", string.Empty), "Logcat_Main");
                    IslemYapildi = true;
                }
                if (checkBox5.Checked && BaslatilmisLoglar.Contains("Events") == false) //Events Log
                {
                    LogBaslatKomutGonder(i, LogTemizle + "adb logcat " + KomutAdb + " -v time -b events", BagliSeriNumaralar[i].ToString().Trim().Replace(" ", string.Empty), HataDuzeyi.ToString(), "Events");
                    IslemYapildi = true;
                }
                if (checkBox6.Checked && BaslatilmisLoglar.Contains("Radio") == false) //Radio Log
                {
                    LogBaslatKomutGonder(i, LogTemizle + "adb logcat " + KomutAdb + " -v time -b radio", BagliSeriNumaralar[i].ToString().Trim().Replace(" ", string.Empty), HataDuzeyi.ToString(), "Radio");
                    IslemYapildi = true;
                }
                if (checkBox7.Checked && BaslatilmisLoglar.Contains("System") == false) //System Log
                {
                    LogBaslatKomutGonder(i, LogTemizle + "adb logcat " + KomutAdb + " -v time -b system", BagliSeriNumaralar[i].ToString().Trim().Replace(" ", string.Empty), HataDuzeyi.ToString(), "System");
                    IslemYapildi = true;
                }
                if (checkBox8.Checked && BaslatilmisLoglar.Contains("Crash") == false) //Crash Log
                {
                    LogBaslatKomutGonder(i, LogTemizle + "adb logcat " + KomutAdb + " -v time -b crash", BagliSeriNumaralar[i].ToString().Trim().Replace(" ", string.Empty), HataDuzeyi.ToString(), "Crash");
                    IslemYapildi = true;
                }

                
                BaslatilmisLoglar.Clear();

                timer1.Enabled = true;
            }

            if (radioButton8.Checked && IslemYapildi == true) { LogTekrar++; }

        }

        public void LogBaslatKomutGonder(int seriSira,String Komut,String Seri, String HataDuzeyi,String HataTuru) {
         
            Process Cmd = new Process();
            ProcessStartInfo Cmd_Bilgi = new ProcessStartInfo();
            String YeniKomut;
            Cmd_Bilgi.FileName = "cmd.exe";
            Cmd_Bilgi.WorkingDirectory = "C:\\Users\\" + KullAdi + "\\Documents\\QaGeneral\\xtool_scripts";

            if (checkBox1.Checked)
            {
                Cmd_Bilgi.WindowStyle = ProcessWindowStyle.Minimized;
            }
            else
            {
                Cmd_Bilgi.WindowStyle = ProcessWindowStyle.Normal;
            }
            
            YeniKomut = Komut.Replace("adb shell", "adb -s " + Seri + " shell").Replace("adb", "adb -s " + Seri);
        
            if (radioButton8.Checked) {

                for (int c = 0; c < BagliSeriNumaralar.Count; c++)
                {

                    if (Directory.Exists(@"" + textBox2.Text + "\\" + Seri.Trim().Replace(" ", string.Empty) + "\\" + LogTekrar.ToString() + "_Tekrar") == false)
                    {
                        Directory.CreateDirectory(@"" + textBox2.Text + "\\" + Seri.Trim().Replace(" ", string.Empty) + "\\" + LogTekrar.ToString() + "_Tekrar");
                    }
                }

            if(HataTuru == "Bugreport")
            {
                  
                Cmd_Bilgi.Arguments = "/c cd " + "\"" + textBox2.Text + "\\" + Seri.Trim().Replace(" ", string.Empty) + "\" & " + YeniKomut;
            }
            else
            {
                Cmd_Bilgi.Arguments = "/c " + YeniKomut + " > " + "\"" + textBox2.Text + "\\" + Seri.Trim().Replace(" ", string.Empty) + "\\" + LogTekrar.ToString() + "_Tekrar" + "\\" + HataDuzeyi + "_" + HataTuru + "_LOG" + ".txt\"";
            }

              
            }else
            {
                if (HataTuru == "Bugreport")
                {
                    Cmd_Bilgi.Arguments = "/c cd " + "\"" + textBox2.Text + "\\" + Seri.Trim().Replace(" ", string.Empty) + "\" & " + YeniKomut;
                }
                else
                {
                    Cmd_Bilgi.Arguments = "/c " + YeniKomut + " > " + "\"" + textBox2.Text + "\\" + Seri.Trim().Replace(" ", string.Empty) + "\\" + HataDuzeyi + "_" + HataTuru + "_LOG" + ".txt\"";
                }
         
            }         
            Cmd.StartInfo = Cmd_Bilgi;
            Cmd.Start();
            AcikCmd.Add(Cmd.Id.ToString());
            TumCmdList.Add(Cmd.Id.ToString());
            BaslatilmisSeriNumaralar.Add(Seri.ToString() + "," + Cmd.Id.ToString() + "," + HataTuru.ToString());                                             
        }
   
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog opfl = new FolderBrowserDialog();
                // OpenFileDialog nesnesi.
                opfl.ShowNewFolderButton = true;
                opfl.ShowDialog();
                textBox2.Text = opfl.SelectedPath;
                this.LogKlasorYol = new DirectoryInfo(opfl.SelectedPath);
                if ((this.textBox2.Text == ""))
                {
                    MessageBox.Show("Lütfen Klasör Seçiniz !");
                    textBox2.Text = "C:\\Users\\" + KullAdi + "\\Desktop\\QaGeneral_Log";
                    return;
                }
             
            }
            catch (Exception ex)
            {
                textBox2.Text = "C:\\Users\\" + KullAdi + "\\Desktop\\QaGeneral_Log";
                MessageBox.Show("Lütfen Klasör Seçiniz !");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (Process islem in Process.GetProcessesByName("cmd"))
            {
                if (AcikCmd.Contains(islem.Id.ToString()) || BugreportCmdList.Contains(islem.Id.ToString()))
                {
                    BugreportCmdList.Remove(islem.Id.ToString());               
                    islem.CloseMainWindow();
                    islem.Close();
                }

            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Saniye = 0;
            Dakika = 0;
            Saat = 0;
            Gun = 0;
            label6.Text = ("Toplam Geçen Süre : 0");        
            radioButton8.Enabled = true;
            radioButton9.Enabled = true;
            foreach (Process islem in Process.GetProcessesByName("cmd"))
            {
                if (TumCmdList.Contains(islem.Id.ToString()))
                {                  
                    islem.CloseMainWindow();
                    islem.Close();
                }

            }
            BaslatilmisSeriNumaralar.Clear();
            AcikCmd.Clear();
            
            label16.Text = "Yapılan İşlem : Log Kapatıldı";
        }

      
        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked)
            {
                textBox1.Text = "0";
                textBox1.Enabled = false;
            }
            else
            {
                textBox1.Text = "20";
                textBox1.Enabled = true;
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)
            {
                radioButton9.Checked = false;
            }
            else
            {

                radioButton9.Checked = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(@"" + textBox2.Text + "\\" + BagliSeriNumaralar[0].ToString());

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                label7.Text = "Not : Bugreport alabilmeniz için Pach Kısmına ADB eklemelisiniz";
                label7.Visible = true;

            }
            else
            {
                label7.Visible = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            KaanOrtak.Arayuz_Ortak.Kaan_Arayuz_Tasarim(this);
            textBox2.Text = "C:\\Users\\" + KullAdi + "\\Desktop\\QaGeneral_Log";
            if (Directory.Exists(@"C:\\Users\\" + KullAdi + "\\Desktop\\QaGeneral_Log") == false)
            {
                Directory.CreateDirectory(@"C:\\Users\\" + KullAdi + "\\Desktop\\QaGeneral_Log");
            }

            pictureBox1.Image = Image.FromFile("C:\\Users\\" + KullAdi + "\\Documents\\QaGeneral\\QaIco.ico");
           
           
        }

        public void timer1_Tick(object sender, EventArgs e)
        {
            Saniye++;

            SurekliTekrarBaslat();

            if (radioButton8.Checked && radioButton9.Checked == false)
            {
                Zamanlayici++;

                if (Zamanlayici >= Convert.ToInt32(textBox1.Text))
                {
                    Zamanlayici = 0;
                    TekrarBaslat();
                }
            }
           
            if ((Saniye >= 60))
            {
                Dakika++;
                Saniye = 0;
            }

            if ((Dakika >= 60))
            {
                Saat++;
                Dakika = 0;
            }

            if ((Saat >= 24))
            {
                Gun++;
                Saat = 0;
            }

            if (radioButton8.Checked)
            {
                label6.Text = ("Toplam Geçen Süre : " + TestSuresiHesap() + " - Tekrar Başlatmaya Kalan Süre : " + (Convert.ToInt32(textBox1.Text) - Zamanlayici).ToString());
            }
            else {

                label6.Text = ("Toplam Geçen Süre : " + TestSuresiHesap());
            }
           
        }

        String TestSuresiHesap()
        {
            if ((Gun > 0))
            {
                return (Gun.ToString() + " Gün "+ Saat.ToString() + " Saat "+ Dakika.ToString() + " Dakika "+ Saniye + " Saniye");
            }
            else if ((Saat > 0))
            {
                return (Saat.ToString() + (" Saat "
                            + (Dakika.ToString() + (" Dakika "
                            + (Saniye.ToString() + " Saniye")))));
            }
            else if ((Dakika > 0))
            {
                return (Dakika.ToString() + (" Dakika "
                            + (Saniye.ToString() + " Saniye")));
            }
            else if ((Saniye > 0))
            {
                return (Saniye.ToString() + " Saniye");
            }
            else
            {
                return "Başlıyor";
            }

        }

        public void TekrarBaslat() {
            CmdIdList.Clear();
            ArrayList SilinecekList = new ArrayList();
            ArrayList GeciciCmdList = new ArrayList();
            ArrayList GeciciGorevList = new ArrayList();
            ArrayList GeciciSeriList = new ArrayList();

            foreach (Process islem in Process.GetProcessesByName("cmd"))
            {
                CmdIdList.Add(islem.Id.ToString());

            }

            for (int i = 0; i < BaslatilmisSeriNumaralar.Count; i++)
            {
                GeciciCmdList.Add(BaslatilmisSeriNumaralar[i].ToString().Split(',')[1]);
                GeciciGorevList.Add(BaslatilmisSeriNumaralar[i].ToString().Split(',')[2]);
                GeciciSeriList.Add(BaslatilmisSeriNumaralar[i].ToString().Split(',')[0]);
            }
             
            for (int i = 0; i < GeciciCmdList.Count; i++)
            {

                if (CmdIdList.Contains(GeciciCmdList[i]) && GeciciGorevList[i].ToString() != "Bugreport")
                {
                    BaslatilmisSeriNumaralar.Remove(GeciciSeriList[i] + "," + GeciciCmdList[i] + "," + GeciciGorevList[i]);                             
                    foreach (Process islem in Process.GetProcessesByName("cmd"))
                    {
                        if (GeciciCmdList[i].ToString() == islem.Id.ToString())
                        {
                            islem.CloseMainWindow();
                            islem.Close();
                        }

                    }

                }
                else if(CmdIdList.Contains(GeciciCmdList[i]) == false)
                {
                 
                    BaslatilmisSeriNumaralar.Remove(GeciciSeriList[i] + "," + GeciciCmdList[i] + "," + GeciciGorevList[i]);
                }
            }
         
            LogAl();
                      
            }

        public void SurekliTekrarBaslat()
        {


            CmdIdList.Clear();
            ArrayList SilinecekList = new ArrayList();
            ArrayList GeciciCmdList = new ArrayList();
            ArrayList GeciciGorevList = new ArrayList();
            ArrayList GeciciSeriList = new ArrayList();

            foreach (Process islem in Process.GetProcessesByName("cmd"))
            {
                CmdIdList.Add(islem.Id.ToString());

            }

            for (int i = 0; i < BaslatilmisSeriNumaralar.Count; i++)
            {
                GeciciCmdList.Add(BaslatilmisSeriNumaralar[i].ToString().Split(',')[1]);
                GeciciGorevList.Add(BaslatilmisSeriNumaralar[i].ToString().Split(',')[2]);
                GeciciSeriList.Add(BaslatilmisSeriNumaralar[i].ToString().Split(',')[0]);
            }



            for (int i = 0; i < GeciciCmdList.Count; i++)
            {
               if (CmdIdList.Contains(GeciciCmdList[i]) == false)
                {

                    //BaslatilmisSeriNumaralar.Remove(GeciciSeriList[i] + "," + GeciciCmdList[i] + "," + GeciciGorevList[i]);

                }
            }
            LogAl();
        }
    }
}

    
