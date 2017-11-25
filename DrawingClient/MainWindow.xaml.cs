using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
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

namespace DrawingClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Stopwatch sw = new Stopwatch();
        long lastFrame = 0;
        static TcpClient client;
        static NetworkStream stream;

        public MainWindow()
        {
            // recieve message
            
            do
            {
                try
                {
                    InputIP ipdia = new InputIP();
                    ipdia.ShowDialog();
                    if (ipdia.close == true)
                    {
                        System.Environment.Exit(0);
                    }
                    client = new TcpClient(ipdia.ip, 5432);
                    stream = client.GetStream();

                    break;
                }
                catch
                {
                }
            } while (true);


            Task.Run(() =>
            {
                while (true)
                {
                    byte[] messagerecieve = new byte[client.ReceiveBufferSize];
                    int read = stream.Read(messagerecieve, 0, client.ReceiveBufferSize);
                    string obj = Encoding.ASCII.GetString(messagerecieve, 0, read);
                    Dispatcher.Invoke(() => recieve(obj));
                }
            });
            // start
            InitializeComponent();
            size_Slider.Value = 5;
            sw.Start();
            canvas.MouseMove += new MouseEventHandler(canvas_MouseMove);
        }
        double lastX = 0;
        double lastY = 0;
        Line line2;



        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton.Equals(MouseButtonState.Pressed) && framePassed())
            {
                double x;
                double y;
                MyLine line = new MyLine();
                x = e.GetPosition(canvas).X;
                y = e.GetPosition(canvas).Y;
                if (x > canvas.Width)
                {
                    x = canvas.Width;
                }
                if (y > canvas.Width)
                {
                    y = canvas.Width;
                }
                if (x < 0)
                {
                    x = 0;
                }
                if (y < 0)
                {
                    y = 0;
                }
                //MessageBox.Show(x+" "+y);
                line.X2 = lastX;
                line.Y2 = lastY;
                lastX = x;
                lastY = y;
                line.X1 = x;
                line.Y1 = y;
                try
                {
                    line.thickness = (byte)size_Slider.Value;
                }
                catch
                {

                }

                if (line.thickness > 20 || line.thickness < 0)
                {
                    line.thickness = 3;
                }
                try
                {
                    line.rgb = new byte[] { (byte)(red_Slider.Value), (byte)(green_Slider.Value ), (byte)(blue_Slider.Value) };
                }
                catch
                {
                    MessageBox.Show("Only valid values are allowed (0-255)");
                }
                line2 = MyLine.newLine(line.X1, line.Y1, line.X2, line.Y2, line.thickness, line.rgb);
                canvas.Children.Add(line2);
                sendLine(line);
            }
        }

        public void recieve(string obj)
        {
            try
            {
                while (true)
                {
                    int index = obj.IndexOf("}}") + 1;
                    string objtemp = obj.Substring(0, index + 1);
                    Envelope unknown = JsonConvert.DeserializeObject<Envelope>(objtemp);


                    if (unknown.type == "MyLine")
                    {
                        MyLine myLine = JsonConvert.DeserializeObject<MyLine>(unknown.obj.ToString());
                        Line line = MyLine.newLine(myLine.X1, myLine.Y1, myLine.X2, myLine.Y2, myLine.thickness, myLine.rgb);
                        canvas.Children.Add(line);
                    }
                    else if (unknown.type == "Message")
                    {
                        Message m = JsonConvert.DeserializeObject<Message>(unknown.obj.ToString());
                        string s = m.user + ": " + m.text + "\n";
                        chatWindow.Text += s;
                    }
                    else
                    {
                        break;
                    }

                    /*
                    MyLine l = null;
                    l = (unknown as MyLine);
                    if (l==null)
                    {
                        MessageBox.Show("This doesn't show");
                    }
                    if (l is MyLine)
                    {
                        MessageBox.Show("This doesn't show either???");
                    }
                    */
                    /*
                    try
                    {
                        unknown = unknown as MyLine;
                    }catch
                    {
                        unknown = unknown as Message;
                    }
                    if (unknown is MyLine)
                    {
                        MessageBox.Show("It's a line");
                    }
                    else if (unknown is Message)
                    {
                        MessageBox.Show("It's a message");
                    }
                    */




                    if (objtemp.Length == obj.Length)
                    {
                        break;
                    }
                    obj = obj.Substring(index + 1, obj.Length - index - 1);
                }
            }
            catch
            {

            }
        }

        public void recieveLine(string obj)
        {
            try
            {
                while (true)
                {
                    int index = obj.IndexOf('}');
                    string objtemp = obj.Substring(0, index + 1);

                    MyLine myLine = JsonConvert.DeserializeObject<MyLine>(objtemp);
                    Line line = MyLine.newLine(myLine.X1, myLine.Y1, myLine.X2, myLine.Y2, myLine.thickness, myLine.rgb);
                    canvas.Children.Add(line);

                    if (objtemp.Length == obj.Length)
                    {
                        break;
                    }
                    obj = obj.Substring(index + 1, obj.Length - index - 1);
                }
            }
            catch
            {

            }



        }

        public void sendLine(MyLine line)
        {
            Envelope e = new Envelope("MyLine", line);
            string obj = JsonConvert.SerializeObject(e);
            // send message
            byte[] message = ASCIIEncoding.ASCII.GetBytes(obj);
            stream.Write(message, 0, message.Length);
        }

        public bool framePassed()
        {
            if (lastFrame + 1000 / 30.0 < sw.ElapsedMilliseconds)
            {
                lastFrame = sw.ElapsedMilliseconds;
                return true;
            }
            return false;
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastX = e.GetPosition(canvas).X;
            lastY = e.GetPosition(canvas).Y;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            if (userName.Text.Length > 3 && userName.Text.Length < 15 && message.Text != "")
            {
                Message m = new Message();
                m.text = message.Text;
                m.user = userName.Text;
                message.Text = "";
                Envelope en = new Envelope("Message", m);
                string obj = JsonConvert.SerializeObject(en);
                // send message
                byte[] messageToSend = ASCIIEncoding.ASCII.GetBytes(obj);
                stream.Write(messageToSend, 0, messageToSend.Length);
            }
            else
            {
                MessageBox.Show("Name must be between 3-15 letters long, message must contain text. Rules ya know");
            }

        }

        private void message_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                send_Click(sender, e);
            }
        }

     //   SolidColorBrush brushColour;
        

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            //Sam.Fill = new SolidColorBrush(Colors.Red);
            try
            {

                SampleColour.Fill = new SolidColorBrush(Color.FromRgb((byte)(red_Slider.Value), (byte)(green_Slider.Value), (byte)(blue_Slider.Value)));
            }
            catch
            {

            }
        }

        private void Slider_Size_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
