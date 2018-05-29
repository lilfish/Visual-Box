using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSCore.CoreAudioAPI;
using System.IO.Ports;


namespace UXU_SoundSystem
{
    public partial class Form1 : Form
    {

        bool isconnected = false;
        bool timer1Bool = false;
        String[] ports;
        SerialPort port;

        public Form1()
        {
            InitializeComponent();
            getAvailablePorst();
            for (int i = 0; i < ports.Length; i++)
            {
                Console.WriteLine(ports[i]);
                porst_comboBox.Items.Add(ports[i]);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }


        private void getAvailablePorst()
        {
            ports = SerialPort.GetPortNames();
        }
        string com = null;
        private void connect_btn_Click(object sender, EventArgs e)
        {
            com = porst_comboBox.GetItemText(porst_comboBox.SelectedItem).ToString();
            if (!String.IsNullOrEmpty(com))
            {
                if (!isconnected)
                {
                    
                    connectToArduino();
                }
                else
                {

                    disconnectArduino();

                }
            }
        }
        private void connectToArduino()
        {
            isconnected = true;

            

            timer2.Start();
            string selectedPort = porst_comboBox.GetItemText(porst_comboBox.SelectedItem);
            port = new SerialPort(selectedPort, 2000000, Parity.None, 8, StopBits.One);
            try
            {
                port.Open();
                port.WriteLine(" STARTEDUP");
                connectButton.Text = "disconnect";
            }
            catch (Exception e)
            {
                port.Close();
                Console.WriteLine(String.Format("Error ({0}) No RF module", e.Message));
                connectButton.Text = "error";
            }

        }
        private void disconnectArduino()
        {

            

            isconnected = false;
            connectButton.Text = "connect";
            port.Close();
        }


        //SOUND SHIT!
        public static MMDevice GetDefaultRenderDevice()
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            }
        }
        public static double IsAudioPlaying(MMDevice device)
        {
            using (var meter = AudioMeterInformation.FromDevice(device))
            {
                if (meter.PeakValue > 0.1)
                {
                    return meter.PeakValue;
                } else
                {
                    return 0;
                }
            }
        }

        double piek = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            ditPiek(IsAudioPlaying(GetDefaultRenderDevice()));
            label1.Text = ((IsAudioPlaying(GetDefaultRenderDevice()) * 100).ToString());
            if (IsAudioPlaying(GetDefaultRenderDevice()) > 0)
            {
                label4.Text = "true";
            } else
            {
                timer3.Start();
            }
        }
        decimal old_test;
        int i = 0;
        bool peekUp = false;
        private void ditPiek(double value)
        {
            decimal test = Convert.ToInt32(value * 100);
            label5.Text = "Treshhold = " + old_test.ToString();


            if (test > old_test && peekUp == false)
            {
                peekUp = true;
                label7.Text = test.ToString();
                label9.Text = (test / 100 * (100 - numericUpDown1.Value)).ToString();
                old_test = (test / 100 * (100 - numericUpDown1.Value));
                Console.WriteLine("Peak");
                panel1.BackColor = Color.FromArgb(0, 255, 0);
                if (isconnected)
                {
                    port.WriteLine(" p");
                   
                }
            }
            if (test < old_test / 100 * (100- numericUpDown1.Value))
            {
                peekUp = false;
                Console.WriteLine("peak Low");
                panel1.BackColor = Color.FromArgb(255, 80, 80);
            }
            

        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            com = porst_comboBox.GetItemText(porst_comboBox.SelectedItem).ToString();
            if (!String.IsNullOrEmpty(com))
            {
                if (!isconnected)
                {
                    connectToArduino();
                }
                else
                {
                    disconnectArduino();
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine(timer1Bool);
            if (!timer1Bool)
            {
                Console.WriteLine(timer1Bool);
                timer1.Start();
                timer1Bool = true;
                button1.Text = "Stop sound receiver";
            }
            else
            {
                label4.Text = "false";
                label1.Text = "0";
                timer1.Stop();
                timer1Bool = false;
                button1.Text = "Start sound receiver";
            }
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            old_test = 0;
            label5.Text = "Treshhold reset";
            peekUp = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            old_test = 0;
            label5.Text = "Treshhold reset";
            peekUp = false;
        }
    }
}
