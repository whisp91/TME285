﻿using AudioLibrary;
using ObjectSerializerLibrary;
using SpeechSynthesisLibrary.FormantSynthesis;
using System;
using System.Collections.Generic;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormatSpeechDemo
{
    public partial class SS1 : Form
    {
        #region Fields
        //private FormantSpecification soundEditorFormantSpecification = null;
        //private FormantSettings soundEditorFormantSettings = null;
        //private FormantSpecification ieaSelectedFormantSpecification = null;
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        #endregion

        public SS1()
        {
            InitializeComponent();
            wordBox.Items.Add("foo");
            sentenceBox.Items.Add("bar");
        }

        private void removeSentenceWord(object sender, MouseEventArgs e)
        {
            int i = sentenceBox.SelectedIndex;
            sentenceBox.Items.RemoveAt(i);
        }

        private void addSentenceWord(object sender, MouseEventArgs e)
        {
            var i = wordBox.SelectedItem;
            sentenceBox.Items.Add(i);
        }

        private void speakSentence(object sender, EventArgs e)
        {
            if (sentenceBox.Items.Count == 0) return;

            List<string> wordList = new List<string>(sentenceBox.Items.Count);
            List<double> silenceList = new List<double>(sentenceBox.Items.Count);

            foreach (var v in sentenceBox.Items)
            {
                string word = v as string;
                wordList.Add(word);
                silenceList.Add(0);
            }

            WAVSound sentence = synthesizer.GenerateWordSequence(wordList, silenceList);

            SoundPlayer soundPlayer = new SoundPlayer();
            sentence.GenerateMemoryStream();
            sentence.WAVMemoryStream.Position = 0; // Manually rewind stream 
            soundPlayer.Stream = null; //TODO varför?
            soundPlayer.Stream = sentence.WAVMemoryStream;
            soundPlayer.PlaySync();
        }

        private void load_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "xml files (*.xml)|*.xml";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    synthesizer = (SpeechSynthesizer)ObjectXmlSerializer.ObtainSerializedObject(openFileDialog.FileName, typeof(SpeechSynthesizer));

                    synthesizer.SpecificationList.Sort((a, b) => a.Name.CompareTo(b.Name));

                    //Load the sounds into list.
                    sentenceBox.Items.Clear();
                    wordBox.Items.Clear();

                    foreach (var wtsm in synthesizer.WordToSoundMappingList)
                        wordBox.Items.Add(wtsm.Word);
                }
            }
        }

        private void singleWord(object sender, EventArgs e)
        {
            var word = wordBox.SelectedItem;
            WAVSound sound = synthesizer.GenerateWord((string)word);

            SoundPlayer soundPlayer = new SoundPlayer();
            sound.GenerateMemoryStream();
            sound.WAVMemoryStream.Position = 0; // Manually rewind stream 
            soundPlayer.Stream = null;
            soundPlayer.Stream = sound.WAVMemoryStream;
            soundPlayer.PlaySync();
        }
    }
}