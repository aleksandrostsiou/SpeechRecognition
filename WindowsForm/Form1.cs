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
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.IO;
using System;
//using System.Security.Cryptography.X509Certificates;

namespace WindowsForm
{
    public partial class Form1 : Form
    {
       
        //Grammar n Response ConfigFile
        string[] grammarFile = File.ReadAllLines(@"C:\Users\alext\OneDrive\Desktop\voiceBot\grammar.txt");
        string[] responseFile = File.ReadAllLines(@"C:\Users\alext\OneDrive\Desktop\voiceBot\response.txt");
        //Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)

        //Speech Synthesis
        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

        //Speech Recognizer
        SpeechRecognizer rec = new SpeechRecognizer();
        
        //Speech Recognition
        public Choices grammarList = new Choices();
        SpeechRecognitionEngine speechRecognitionEngine = new SpeechRecognitionEngine();
 

        public Form1()
        {
            //Init Grammar
            grammarList.Add(grammarFile);
            Grammar grammar = new Grammar(new GrammarBuilder(grammarList));

            try
            {
                speechRecognitionEngine.RequestRecognizerUpdate();
                speechRecognitionEngine.LoadGrammarAsync(grammar);
                speechRecognitionEngine.SetInputToDefaultAudioDevice();
                speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
                speechRecognitionEngine.SpeechRecognized += SpeechRecognizedEvent;//Subscriber speech recognized
                speechRecognitionEngine.SpeechRecognitionRejected += SpeechNotRecognizedEvent;//Subscriber speech not recognized
            }
            catch //*********Exeptions***********
            {
                return ;
            }
            
            //Custom Speech Sythesis Settings 
            speechSynthesizer.SelectVoiceByHints(VoiceGender.Female);
            
            InitializeComponent();

            //Disposing resourses
            speechSynthesizer.Dispose();
            rec.Dispose();
            speechRecognitionEngine.Dispose();
        }
        
        public int timesRejected = 0;
        private void SpeechNotRecognizedEvent(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            List<string> rejectResponses = new List<string>() { "i don't understand", "Please repeat", "Please rephrase" };
            Random r = new Random();
            if (timesRejected <= 4)
            {
                Say(rejectResponses[r.Next(rejectResponses.Count)]);
                timesRejected++;
            }
            else
            {
                Say("Are you stupid");
            }
            
        }

        public void Say(string text) => speechSynthesizer.SpeakAsync(text);
        //Overloading Say
        public void Say(string text1,string text2) => speechSynthesizer.SpeakAsync(text1+text2);
        
        private void SpeechRecognizedEvent(object sender, SpeechRecognizedEventArgs e)
        {
            Random r = new Random();
            string result = e.Result.Text;
            int indexResponse = Array.IndexOf(grammarFile, result);

            //Multiple responses splitting from text
            List<string> multipleResponse = responseFile[indexResponse].Replace('+', ' ').Split('/').ToList();

            //Multiple responses
            if (responseFile[indexResponse].Contains("+"))
            {
                Say(multipleResponse[r.Next(multipleResponse.Count)]);
            }
            //single response
            else
            {
                Say(responseFile[indexResponse]);
            }

            //if (e.Result.Text == "search")
            //{
            //    int index = 0;
            //    string url = e.Result.Text.Replace(" ", "+").Remove(index);
            //    LauncheGoogleChrome(url);
            //}
        }

        //public void LauncheGoogleChrome(string url)
        //{
        //    Process.Start("Chrome.exe", url);
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        ~Form1()
        {
            speechRecognitionEngine.Dispose();
        }
    }
}
