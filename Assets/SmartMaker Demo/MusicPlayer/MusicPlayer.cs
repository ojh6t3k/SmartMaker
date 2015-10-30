using UnityEngine;
using System.Collections.Generic;
using SmartMaker;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public GraphDrawer graphDrawer;
    public AudioClip[] musics;
    public SignalController beatLED;

    private AudioSource _audioSource;
    private int _resolution = 8192;
    private float[] _spectrum;
    private int _history = 256;
    private float[] _power;
    private float[] _beatSum;
    private float[] _beat;
    private float _unitFrequency;
    private float _maxPower;
    private float _maxBeat;
    private int _20Hz;
    private int _60Hz;
    private int _20kHz;
    private int _150Hz;
    private int _250Hz;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _spectrum = new float[_resolution];
        _power = new float[_history];
        _beatSum = new float[_history];
        _beat = new float[_history];

        _unitFrequency = 24000 / _resolution;
        _20Hz = (int)(20 / _unitFrequency);
        _60Hz = (int)(60 / _unitFrequency);        
        _150Hz = (int)(150 / _unitFrequency);
        _250Hz = (int)(250 / _unitFrequency);
        _20kHz = (int)(20000 / _unitFrequency);
        _20kHz = Mathf.Min(_20kHz, _resolution - 1);

        beatLED.index = 0;
        beatLED.speed = 3f;
    }

    // Use this for initialization
    void Start ()
    {        
        Play(0);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

            for (int i = 0; i < (_history - 1); i++)
            {
                _power[i] = _power[i + 1];
                _beatSum[i] = _beatSum[i + 1];
                _beat[i] = _beat[i + 1];
            }

            float sum = 0;
            int count = 0;
            for (int i = _20Hz; i <= _20kHz; i++, count++)
                sum += _spectrum[i];
            float power = sum / count;
            _maxPower = Mathf.Max(_maxPower, power);
            _power[_history - 1] = power / _maxPower;

            sum = 0;
            count = 0;
            for (int i = _60Hz; i <= _20kHz; i++, count++)
                sum += _spectrum[i];
            _beatSum[_history - 1] = sum;

            float diff = _beatSum[_history - 1] - _beatSum[_history - 2];
            if (diff > 0.45f)
            {
                _beat[_history - 1] = diff;
                _maxBeat = Mathf.Max(_maxBeat, diff);
            }
            else
                _beat[_history - 1] = 0f;

            if(_beat[_history - 1] > 0f)
            {
                float multiplier = _beat[_history - 1] / _maxBeat;
                if (!beatLED.isPlaying || multiplier > beatLED.multiplier)
                    beatLED.multiplier = multiplier;
                beatLED.Play();
            }
        }
    }

    public void Play(int index)
    {
        if (index < 0 || index >= musics.Length)
            return;

        if (graphDrawer != null)
        {
            graphDrawer.graphs.Clear();

            GraphDrawer.Graph graph;

            graph = new GraphDrawer.Graph();
            graph.name = "Beat";
            graph.data = _beat;
            graph.color = Color.magenta;
            graphDrawer.graphs.Add(graph);

            graph = new GraphDrawer.Graph();
            graph.name = "Power";
            graph.data = _power;
            graph.color = Color.blue;
            graphDrawer.graphs.Add(graph);

            graph = new GraphDrawer.Graph();
            graph.name = "Spectrum";
            graph.data = _spectrum;
            graph.color = Color.green;
            graphDrawer.graphs.Add(graph);

            _maxPower = 0f;
            _maxBeat = 0f;
        }

        _audioSource.clip = musics[index];
        _audioSource.Play();
    }

    public void Pause()
    {
        _audioSource.Pause();
    }

    public void Resume()
    {
        _audioSource.UnPause();
    }

    public void Stop()
    {
        _audioSource.Stop();
    }
}
