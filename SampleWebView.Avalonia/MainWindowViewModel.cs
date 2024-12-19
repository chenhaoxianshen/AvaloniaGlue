using System;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using WebViewControl;

namespace SampleWebView.Avalonia {
    class MainWindowViewModel : ReactiveObject {

        private string address;
        private string currentAddress;

        public MainWindowViewModel(WebView webview) {
            Address = CurrentAddress = "http://www.baidu.com/";

            NavigateCommand = ReactiveCommand.Create(() => {
                CurrentAddress = Address;
            });

            ShowDevToolsCommand = ReactiveCommand.Create(() => {
                webview.ShowDeveloperTools();
            });

            CutCommand = ReactiveCommand.Create(() => {
                webview.EditCommands.Cut();
            });

            CopyCommand = ReactiveCommand.Create(() => {
                webview.EditCommands.Copy();
            });

            PasteCommand = ReactiveCommand.Create(() => {
                webview.EditCommands.Paste();
            });

            UndoCommand = ReactiveCommand.Create(() => {
                webview.EditCommands.Undo();
            });

            RedoCommand = ReactiveCommand.Create(() => {
                webview.EditCommands.Redo();
            });

            SelectAllCommand = ReactiveCommand.Create(() => {
                webview.EditCommands.SelectAll();
            });

            DeleteCommand = ReactiveCommand.Create(() => {
                webview.EditCommands.Delete();
            });
            
            BackCommand = ReactiveCommand.Create(() => {
                webview.GoBack();
            });
            
            ForwardCommand = ReactiveCommand.Create(() => {
                webview.GoForward();
            });

            //SoxPlay = ReactiveCommand.Create(() => {

            //    var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stop.WAV");
            //    var sox = new SoxHelper();
            //    sox.targetPath = filePath;

            //    Task.Run(() => {
            //        sox.PlaySoxVideo();

            //    });
            //    Console.WriteLine("播放完成");
            //});

            PropertyChanged += OnPropertyChanged;
        }

        public void soxPlay() {
             
            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stop.WAV");
            var sox = new SoxHelper();
            sox.targetPath = filePath;

            Task.Run(() => {
                sox.PlaySoxVideo();

            });
            Console.WriteLine("播放完成");
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentAddress)) {
                Address = CurrentAddress;
            }
        }

        public string Address {
            get => address;
            set => this.RaiseAndSetIfChanged(ref address, value);
        }

        public string CurrentAddress {
            get => currentAddress;
            set => this.RaiseAndSetIfChanged(ref currentAddress, value);
        }

        public ReactiveCommand<Unit, Unit> NavigateCommand { get; }

        public ReactiveCommand<Unit, Unit> ShowDevToolsCommand { get; }

        public ReactiveCommand<Unit, Unit> CutCommand { get; }

        public ReactiveCommand<Unit, Unit> CopyCommand { get; }

        public ReactiveCommand<Unit, Unit> PasteCommand { get; }

        public ReactiveCommand<Unit, Unit> UndoCommand { get; }

        public ReactiveCommand<Unit, Unit> RedoCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectAllCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        
        public ReactiveCommand<Unit, Unit> ForwardCommand { get; }

        //public ReactiveCommand<Unit, Unit> SoxPlay { get; }
    }
}
