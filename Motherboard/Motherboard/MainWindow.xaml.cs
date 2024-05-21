using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



public class Component
{
    public double Voltage { get; set; }
    public double Resistance { get; set; }
    public string tester = "OK";
    public int oscilloscope = 1;

    public string Tester
    {
        get { return tester; }
        set { tester = value; }
    }

    public int Oscilloscope
    {
        get { return oscilloscope; }
        set { oscilloscope = value; }
    }

    public Component(double voltage)
    {
        Voltage = voltage;
    }

    public Component(double voltage, double resistance)
    {
        Voltage = voltage;
        Resistance = resistance;
    }

    public virtual double GenerateErrorVoltage()
    {
        return 0;
    }

    public virtual double GenerateErrorResistance()
    {
        return 0;
    }

    public virtual int GenerateErrorSignal()
    {
        return 0;
    }

    public virtual string GenerateErrorTester()
    {
        return "1 1 1 1 0 1 0 1";
    }

}

public class CMOS : Component
{
    public CMOS(double voltage) : base(voltage)
    {
    }

    public override double GenerateErrorVoltage()
    {
        return 10;
    }

}

public class Line : Component
{
    public Line(double voltage) : base(voltage)
    {
    }

    public override double GenerateErrorVoltage()
    {
        return Voltage * 0.1;
    }

}

public class D : Component
{
    public D(double voltage) : base(voltage)
    {
    }

    public override double GenerateErrorVoltage()
    {
        return 133;
    }
}

public class BIOS : Component
{
    public BIOS(double voltage) : base(voltage)
    {
    }

    public override int GenerateErrorSignal()
    {
        return 0;
    }

}

public class RTC : Component
{
    public RTC(double voltage) : base(voltage)
    {
    }

    public override int GenerateErrorSignal()
    {
        return 0;
    }
}

public class GFX : Component
{
    public GFX(double voltage, string Seq) : base(voltage)
    {
        Tester = Seq;
    }

    public override string GenerateErrorTester()
    {
        return "1 1 1 1 0 1 0 1";
    }
}

public class Choke : Component
{
    public Choke(double voltage) : base(voltage)
    {
    }
    public override int GenerateErrorSignal()
    {
        return 0;
    }
}

// Список неисправностей

// 7. Напря;ение батарейки - v
// 1. Напря;ении линии 3.3V - v
// 2. Напря;ении линии 5V - v
// 3. Напря;ении линии 12V - v
// 8. Южный мост (D+, D-) - v
// 4. BIOS (есть сигнал / нет сигнала на ножках) - осц
// 5. RTC (Правильная частота 32768Гц у кв. резонатора) - осц
// 6. Нет изобра;ения (обрыв на линях передачи данных на разъеме видеокарты) - тестер
// 9. Короткое замыкание (сопротивление компонента близко к 0) - r
// 10. Проверить сигнал на дроселях (везде график дол;ен быть одинаковый 0 1) - осц

namespace Motherboard
{
    public partial class MainWindow : Window
    {
        private int randomNumber;

        public MainWindow()
        {
            InitializeComponent();
            GenerateRandomNumber();

            Component cmos = new CMOS(4.1) { Resistance = 10000 };
            Component line3 = new Line(3.3) { Resistance = 5 };
            Component line5 = new Line(5) { Resistance = 5 };
            Component line12 = new Line(12) { Resistance = 5 };
            Component bios = new BIOS(3) { Resistance = 1000 };
            Component rtc = new RTC(3) { Resistance = 1000 };
            Component gfx = new GFX(12, "1 1 1 1 1 1 1 1") { Resistance = 50 };
            Component choke1 = new Choke(1) { Resistance = 0.1 };
            Component choke2 = new Choke(1) { Resistance = 0.1 };
            Component choke3 = new Choke(1) { Resistance = 0.1 };
            Component choke4 = new Choke(1) { Resistance = 0.1 };
            Component choke5 = new Choke(1) { Resistance = 0.1 };
            Component choke6 = new Choke(1) { Resistance = 0.1 };
            Component choke7 = new Choke(1) { Resistance = 0.1 };
            Component P1Dp = new D(578) { Resistance = 1000 };
            Component P1Dm = new D(577) { Resistance = 1000 };
            Component P2Dp = new D(580) { Resistance = 1000 };
            Component P2Dm = new D(579) { Resistance = 1000 };

            cbGND.Tag = "GND";

            btnCMOS.Tag = cmos;
            btnCMOS.Click += Show_voltage;
            cbRcmos.Tag = cmos;

            btnLine3.Tag = line3;
            btnLine3.Click += Show_voltage;
            cbRl3.Tag = line3;

            btnLine5.Tag = line5;
            btnLine5.Click += Show_voltage;
            cbRl5.Tag = line5;

            btnLine12.Tag = line12;
            btnLine12.Click += Show_voltage;
            cbRl12.Tag = line12;

            btnBIOS.Tag = bios;
            btnBIOS.Click += Show_osc;
            cbRbios.Tag = bios;

            btnRTC.Tag = rtc;
            btnRTC.Click += Show_osc;
            cbRrtc.Tag = rtc;

            btnGFX.Tag = gfx;
            btnGFX.Click += Show_tester;
            cbRgfx.Tag = gfx;

            btnChoke1.Tag = choke1;
            btnChoke1.Click += Show_osc;
            btnChoke1.Click += ToggleSinImage;

            btnChoke2.Tag = choke2;
            btnChoke2.Click += Show_osc;
            btnChoke2.Click += ToggleSinImage;

            btnChoke3.Tag = choke3;
            btnChoke3.Click += Show_osc;
            btnChoke3.Click += ToggleSinImage;

            btnChoke4.Tag = choke4;
            btnChoke4.Click += Show_osc;

            btnChoke5.Tag = choke5;
            btnChoke5.Click += Show_osc;
            btnChoke5.Click += ToggleSinImage;

            btnChoke6.Tag = choke6;
            btnChoke6.Click += Show_osc;
            btnChoke6.Click += ToggleSinImage;

            btnChoke7.Tag = choke7;
            btnChoke7.Click += Show_osc;
            btnChoke7.Click += ToggleSinImage;

            btnP1dp.Tag = P1Dp;
            btnP1dp.Click += Show_voltage;

            btnP1dm.Tag = P1Dm;
            btnP1dm.Click += Show_voltage;

            btnP2dp.Tag = P2Dp;
            btnP2dp.Click += Show_voltage;

            btnP2dm.Tag = P2Dm;
            btnP2dm.Click += Show_voltage;

            btnCheckR.Click += Check_resistance;

            switch (randomNumber)
            {
                case 1:
                    line3.Voltage = line3.GenerateErrorVoltage();
                    break;
                case 2:
                    line5.Voltage = line5.GenerateErrorVoltage();
                    break;
                case 3:
                    line12.Voltage = line12.GenerateErrorVoltage();
                    break;
                case 4:
                    bios.oscilloscope = bios.GenerateErrorSignal();
                    btnBIOS.Click += ToggleNoSignalImage;
                    break;
                case 5:
                    rtc.oscilloscope = rtc.GenerateErrorSignal();
                    btnRTC.Click += ToggleNoSignalImage;
                    break;
                case 6:
                    gfx.Tester = gfx.GenerateErrorTester();
                    break;
                case 7:
                    cmos.Voltage = cmos.GenerateErrorVoltage();
                    break;
                case 8:
                    P1Dm.Voltage = P1Dm.GenerateErrorVoltage();
                    break;
                case 9:
                    line12.Resistance = 0.03;
                    break;
                case 10:
                    choke4.Oscilloscope = choke4.GenerateErrorSignal();
                    btnChoke4.Click += ToggleNoSignalImage;
                    break;
            }
            if (randomNumber != 4)
            {
                btnBIOS.Click += ToggleBiosImage;
            }
            if (randomNumber != 5)
            {
                btnRTC.Click += ToggleRTCImage;
            }
            if (randomNumber != 10)
            {
                btnChoke4.Click += ToggleSinImage;
            }
        }

        private void ToggleBiosImage(object sender, RoutedEventArgs e)
        {
            HideAllImages();
            BIOSImage.Visibility = Visibility.Visible;
        }

        private void ToggleSinImage(object sender, RoutedEventArgs e)
        {
            HideAllImages();
            SINImage.Visibility = Visibility.Visible;
        }

        private void ToggleRTCImage(object sender, RoutedEventArgs e)
        {
            HideAllImages();
            RTCImage.Visibility = Visibility.Visible;
        }

        private void ToggleNoSignalImage(object sender, RoutedEventArgs e)
        {
            HideAllImages();
            NoSignalImage.Visibility = Visibility.Visible;
        }

        private void HideAllImages()
        {
            NoSignalImage.Visibility = Visibility.Hidden;
            RTCImage.Visibility = Visibility.Hidden;
            SINImage.Visibility = Visibility.Hidden;
            BIOSImage.Visibility = Visibility.Hidden;
        }

        private void GenerateRandomNumber()
        {
            Random rand = new Random();
            randomNumber = rand.Next(1, 11);
            //randomNumber = 5;
            textProblem.Text = randomNumber.ToString();
           
        }

        private void BtnGFX_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Show_tester(object sender, RoutedEventArgs e)
        {
            Component component = (Component)((Button)e.OriginalSource).Tag;
            textLabel.Text = component.Tester.ToString();
        }

        private void Show_voltage(object sender, RoutedEventArgs e)
        {
            Component component = (Component)((Button)e.OriginalSource).Tag;
            textLabel.Text = component.Voltage.ToString() + "V";
        }

        private void Show_osc(object sender, RoutedEventArgs e)
        {
            Component component = (Component)((Button)e.OriginalSource).Tag;
            textLabel.Text = component.Oscilloscope.ToString() + " signal";
        }

        private void Check_resistance(object sender, RoutedEventArgs e)
        {
            List<CheckBox> activeCheckBoxes = new List<CheckBox>();

            foreach (UIElement element in MainRoot.Children)
            {
                if (element is CheckBox checkBox && checkBox.IsChecked == true)
                {
                    activeCheckBoxes.Add(checkBox);
                }
            }

            if (activeCheckBoxes.Count == 2)
            {
                if (activeCheckBoxes[0].Content.ToString() == "GND")
                {
                    textLabel.Text = ((Component)activeCheckBoxes[1].Tag).Resistance.ToString() + "Ом";
                } else if (activeCheckBoxes[1].Content.ToString() == "GND") 
                {
                    textLabel.Text = ((Component)activeCheckBoxes[0].Tag).Resistance.ToString() + "Ом";
                } else
                {
                    textLabel.Text = "GOOD";
                }
            }
            else
            {
                textLabel.Text = "Сопротивление можно измерять только у двух компонентов";
            }
        }

        private async void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedError = selectedItem.Content.ToString();

                switch (randomNumber)
                {
                    case 1:
                        if (selectedError == "Неправильное напряжение линии 3.3V")
                        {
                            textLabel.Text = "You WIN";
                        }
                        break;
                    case 2:
                        if (selectedError == "Неправильное напряжение линии 5V")
                        {
                            textLabel.Text = "You WIN";
                        }
                        break;
                    case 3:
                        if (selectedError == "Неправильное напряжение линии 12V")
                        {
                            textLabel.Text = "You WIN";
                        }
                        break;
                    case 4:
                        if (selectedError == "Нет сигнала на BIOS")
                        {
                            textLabel.Text = "You WIN";
                        }
                        break;
                    case 5:
                        if (selectedError == "Нет сигнала на RTC")
                        {
                            textLabel.Text = "You WIN";
                        }
                        break;
                    case 6:
                        if (selectedError == "Обрыв на линиях разъема видеокарты")
                        {
                            textLabel.Text = "You WIN";
                        }
                        break;
                    case 7:
                        if (selectedError == "Неправильное напряжение CMOS")
                        {
                            textLabel.Text = "You WIN";
                        }
                        break;
                    case 8:
                        if (selectedError == "Неисправен южный мост")
                        {
                            textLabel.Text = "You WIN";
                        }
                        break;
                    case 9:
                        if (selectedError == "Короткое замыкание на линии 12V")
                        {
                            textLabel.Text = "You WIN";
                        }
                        break;
                    case 10:
                        if (selectedError == "Различаются осцилограммы на дроселях")
                        {
                            textLabel.Text = "You WIN";
                        }
                        break;
                }

                if (textLabel.Text == "You WIN")
                {
                    await Task.Delay(3000);
                    RestartApplication();
                }
                else
                {
                    textLabel.Text = "Вы уволены";
                    await Task.Delay(3000);
                    RestartApplication();
                }
            }
        }

        private void RestartApplication()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

    }
}
