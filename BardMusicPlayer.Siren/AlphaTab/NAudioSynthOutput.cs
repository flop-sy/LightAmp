﻿#region

using System;
using BardMusicPlayer.Siren.AlphaTab.Audio.Synth;
using BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Ds;
using NAudio.CoreAudioApi;
using NAudio.Wave;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab;

/// <summary>
///     A <see cref="ISynthOutput" /> implementation that uses NAudio to play the
///     sound via WasapiOut.
/// </summary>
internal sealed class NAudioSynthOutput : WaveProvider32, ISynthOutput, IDisposable
{
    private const int BufferSize = 4096;

    private const int PreferredSampleRate = 48000;
    private readonly byte _bufferCount;
    private readonly int _bufferCountSize;

    private readonly WasapiOut _context;
    private readonly MMDevice _device;
    private readonly byte _latency;
    private CircularSampleBuffer _circularBuffer;
    private bool _finished;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NAudioSynthOutput" /> class.
    /// </summary>
    public NAudioSynthOutput(MMDevice device, byte bufferCount, byte latency)
        : base(PreferredSampleRate, 2)
    {
        _device = device;
        _context = new WasapiOut(device, AudioClientShareMode.Shared, false, _latency);
        _latency = latency;
        _bufferCount = bufferCount;
        _bufferCountSize = _bufferCount / 2 * BufferSize;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Close();
    }

    /// <inheritdoc />
    public int SampleRate => PreferredSampleRate;

    /// <inheritdoc />
    public void Activate()
    {
    }

    /// <inheritdoc />
    public void Open()
    {
        _finished = false;
        _circularBuffer = new CircularSampleBuffer(BufferSize * _bufferCount);
        _context.Init(this);

        Ready();
    }

    /// <inheritdoc />
    public void Play()
    {
        RequestBuffers();
        _finished = false;
        _context.Play();
    }

    /// <inheritdoc />
    public void Pause()
    {
        _context.Pause();
    }

    /// <inheritdoc />
    public void Stop()
    {
        _finished = true;
        _context.Stop();
        _circularBuffer.Clear();
    }

    /// <inheritdoc />
    public void SequencerFinished()
    {
        _finished = true;
    }

    /// <inheritdoc />
    public void AddSamples(float[] f)
    {
        _circularBuffer.Write(f, 0, f.Length);
    }

    /// <inheritdoc />
    public void ResetSamples()
    {
        _circularBuffer.Clear();
    }

    /// <inheritdoc />
    public event Action Ready;

    /// <inheritdoc />
    public event Action<int> SamplesPlayed;

    /// <inheritdoc />
    public event Action SampleRequest;

    /// <inheritdoc />
    public event Action Finished;

    /// <summary>
    ///     Closes the synth output and disposes all resources.
    /// </summary>
    public void Close()
    {
        _finished = true;
        _context.Stop();
        _circularBuffer.Clear();
        _context.Dispose();
    }

    private void RequestBuffers()
    {
        if (_circularBuffer.Count >= _bufferCountSize || SampleRequest == null) return;

        for (var i = 0; i < _bufferCount / 2; i++) SampleRequest();
    }

    /// <inheritdoc />
    public override int Read(float[] buffer, int offset, int count)
    {
        if (_circularBuffer.Count < count)
        {
            if (_finished) Finished();
        }
        else
        {
            var read = new float[count];
            _circularBuffer.Read(read, 0, read.Length);

            for (var i = 0; i < count; i++) buffer[offset + i] = read[i];

            var samples = count / 2;
            SamplesPlayed(samples);
        }

        if (!_finished) RequestBuffers();

        return count;
    }
}