// made by xerox

using System;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace StresTesterCPU
{
    public partial class MainWindow : Window
    {
        private static volatile bool running = false;
        private CancellationTokenSource cancellationTokenSource;
        private Thread stressThread;

        public MainWindow()
        {
            InitializeComponent();
            Stresser.Click += Stresser_Click;
            stressThread = new Thread(StartStressTest);
            InitializeCpuCounter();
        }

        private void InitializeCpuCounter()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private async void Stresser_Click(object sender, RoutedEventArgs e)
        {
            if (!running)
            {
                running = true;
                MessageBox.Show("Stress-Test wurde gestartet!", "Stress Test", MessageBoxButton.OK, MessageBoxImage.Information);
                cancellationTokenSource = new CancellationTokenSource();
                await StartStressTestAsync(cancellationTokenSource.Token);
            }
        }

        private void StartStressTest()
        {
            int n = 99999; // Anzahl Threads

            for (int i = 0; i < n; i++)
            {
                Thread burnThread = new Thread(Burn);
                burnThread.Start();
            }

            Thread.Sleep(15000); // Kürzere Wartezeit

            running = false;
        }

        private async Task StartStressTestAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => StartStressTest(), cancellationToken);
        }

        private void Burn()
        {
            while (running)
            {
                long primeCount = 0;
                long candidate = 2;

                while (primeCount < 100000)
                {
                    bool isPrime = true;
                    for (long divisor = 2; divisor <= Math.Sqrt(candidate); divisor++)
                    {
                        if (candidate % divisor == 0)
                        {
                            isPrime = false;
                            break;
                        }
                    }

                    if (isPrime)
                    {
                        primeCount++;
                    }

                    candidate++;
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            running = false;
            MessageBox.Show("Stress-Test wurde geschlossen!", "Stress Test", MessageBoxButton.OK, MessageBoxImage.Information);

            cancellationTokenSource?.Cancel();
            Application.Current.Shutdown();
        }
    }
}
