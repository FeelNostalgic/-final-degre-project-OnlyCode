using System.Collections;
using UnityEngine;
using MidiPlayerTK;
using Proyecto.Manager;
/* 
 *Autor: David Báez
 */
namespace Proyecto.MidiPlayer
{
    public class MidiPlayer : Singleton<MidiPlayer>
    {
        private MidiStreamPlayer _midiStreamPlayer;
        private MPTKEvent _mptkEvent;

        void Awake()
        {
            _midiStreamPlayer = GetComponent<MidiStreamPlayer>();
            //StartCoroutine(TestNotes());
        }

        void Start()
        {
            StartCoroutine(InitializeMidiStreamer());
        }

        public void ChangeInstrument(int selectionIndex)
        {
            _midiStreamPlayer.MPTK_ChannelForcedPresetSet(0, GetInstrumentIndex(selectionIndex));
        }

        private IEnumerator InitializeMidiStreamer()
        {
            yield return new WaitForEndOfFrame();
            _midiStreamPlayer = FindObjectOfType<MidiStreamPlayer>();
            _midiStreamPlayer.MPTK_ChannelForcedPresetSet(0, 0);
            _midiStreamPlayer.MPTK_EnableChangeTempo = true;
            _midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent() // Desviaci�n de pitch (a 8192 est� centrada)
            {
                Command = MPTKCommand.PitchWheelChange,
                Value = 8192, // Value between 0 and 16383, center at 8192 (no change)
            });
            UIManager.Instance.SetIntrumentOnStart();
        }

        IEnumerator TestNotes()
        {
            yield return new WaitForSeconds(1);
            PlayNote(60, 5000);
        }

        public void PlayNote(int midi, long duration)
        {
            //Debug.Log("Play Midi: " + midi + " Duration: " + duration);
            _mptkEvent = new MPTKEvent()
            {
                Channel = 0, // Between 0 and 15
                Duration = duration, // Infinite
                Value = midi, // Between 0 and 127, with 60 plays a C4
                Velocity = 100, // Max 127           
            };
            _midiStreamPlayer.MPTK_PlayEvent(_mptkEvent);
        }
        
        void Stop()
        {
            _midiStreamPlayer.MPTK_StopEvent(_mptkEvent);
        }

        private int GetInstrumentIndex(int selectionIndex)
        {
            switch (selectionIndex)
            {
                case 0: return 0;
                case 1: return 6; //harpsichord
                case 2: return 11; //Vibraphono
                case 3: return 19; //Pipe Organ
                case 4: return 22; //Harmonica
                case 5: return 29; //Overdrive Guitar
                case 6: return 40; //Violin
                case 7: return 52; //Concept choir
                case 8: return 68; //Oboe
                case 9: return 72; //Piccolo
                default: return -1;
            }
        }
    }
}