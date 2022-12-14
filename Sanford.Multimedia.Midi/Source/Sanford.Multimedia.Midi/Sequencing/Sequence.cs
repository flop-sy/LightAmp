#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

#endregion

namespace Sanford.Multimedia.Midi;

/// <summary>
///     Represents a collection of Tracks.
/// </summary>
public sealed class Sequence : IComponent, ICollection<Track>
{
    #region IEnumerable<Track> Members

    public IEnumerator<Track> GetEnumerator()
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        #endregion

        return tracks.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        #endregion

        return tracks.GetEnumerator();
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
        #region Guard

        if (disposed) return;

        #endregion

        loadWorker.Dispose();
        saveWorker.Dispose();

        disposed = true;

        var handler = Disposed;

        handler?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Sequence Members

    #region Fields

    // The collection of Tracks for the Sequence.
    private List<Track> tracks = new();

    // The Sequence's MIDI file properties.
    private MidiFileProperties properties = new();

    private readonly BackgroundWorker loadWorker = new();

    private readonly BackgroundWorker saveWorker = new();

    private bool disposed;

    #endregion

    #region Events

    public event EventHandler<AsyncCompletedEventArgs> LoadCompleted;

    public event ProgressChangedEventHandler LoadProgressChanged;

    public event EventHandler<AsyncCompletedEventArgs> SaveCompleted;

    public event ProgressChangedEventHandler SaveProgressChanged;

    #endregion

    #region Construction

    /// <summary>
    ///     Initializes a new instance of the Sequence class.
    /// </summary>
    public Sequence()
    {
        InitializeBackgroundWorkers();
    }

    /// <summary>
    ///     Initializes a new instance of the Sequence class with the specified division.
    /// </summary>
    /// <param name="division">
    ///     The Sequence's division value.
    /// </param>
    public Sequence(int division)
    {
        properties.Division = division;
        properties.Format = 1;

        InitializeBackgroundWorkers();
    }

    /// <summary>
    ///     Initializes a new instance of the Sequence class with the specified
    ///     file name of the MIDI file to load.
    /// </summary>
    /// <param name="fileName">
    ///     The name of the MIDI file to load.
    /// </param>
    public Sequence(string fileName)
    {
        InitializeBackgroundWorkers();

        Load(fileName);
    }

    /// <summary>
    ///     Initializes a new instance of the Sequence class with the specified
    ///     file stream of the MIDI file to load.
    /// </summary>
    /// <param name="fileStream">
    ///     The stream of the MIDI file to load.
    /// </param>
    public Sequence(Stream fileStream)
    {
        InitializeBackgroundWorkers();

        Load(fileStream);
    }

    private void InitializeBackgroundWorkers()
    {
        loadWorker.DoWork += LoadDoWork;
        loadWorker.ProgressChanged += OnLoadProgressChanged;
        loadWorker.RunWorkerCompleted += OnLoadCompleted;
        loadWorker.WorkerReportsProgress = true;

        saveWorker.DoWork += SaveDoWork;
        saveWorker.ProgressChanged += OnSaveProgressChanged;
        saveWorker.RunWorkerCompleted += OnSaveCompleted;
        saveWorker.WorkerReportsProgress = true;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Loads a MIDI file into the Sequence.
    /// </summary>
    /// <param name="fileName">
    ///     The MIDI file's name.
    /// </param>
    public void Load(string fileName)
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        if (IsBusy) throw new InvalidOperationException();

        if (fileName == null) throw new ArgumentNullException(nameof(fileName));

        #endregion

        var stream = new FileStream(fileName, FileMode.Open,
            FileAccess.Read, FileShare.Read);

        using (stream)
        {
            var newProperties = new MidiFileProperties();
            var reader = new TrackReader();
            var newTracks = new List<Track>();

            newProperties.Read(stream);

            for (var i = 0; i < newProperties.TrackCount; i++)
            {
                reader.Read(stream);
                newTracks.Add(reader.Track);
            }

            properties = newProperties;
            tracks = newTracks;
        }

        #region Ensure

        Debug.Assert(Count == properties.TrackCount);

        #endregion
    }

    /// <summary>
    ///     Loads a MIDI stream into the Sequence.
    /// </summary>
    /// <param name="fileStream">
    ///     The MIDI file's stream.
    /// </param>
    public void Load(Stream fileStream)
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        if (IsBusy) throw new InvalidOperationException();

        if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));

        #endregion

        using (fileStream)
        {
            var newProperties = new MidiFileProperties();
            var reader = new TrackReader();
            var newTracks = new List<Track>();

            newProperties.Read(fileStream);

            for (var i = 0; i < newProperties.TrackCount; i++)
            {
                reader.Read(fileStream);
                newTracks.Add(reader.Track);
            }

            properties = newProperties;
            tracks = newTracks;
        }

        #region Ensure

        Debug.Assert(Count == properties.TrackCount);

        #endregion
    }

    public void LoadAsync(string fileName)
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        if (IsBusy) throw new InvalidOperationException();

        if (fileName == null) throw new ArgumentNullException(nameof(fileName));

        #endregion

        loadWorker.RunWorkerAsync(fileName);
    }

    public void LoadAsyncCancel()
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        #endregion

        loadWorker.CancelAsync();
    }

    /// <summary>
    ///     Saves the Sequence as a MIDI file.
    /// </summary>
    /// <param name="fileName">
    ///     The name to use for saving the MIDI file.
    /// </param>
    public void Save(string fileName)
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        if (fileName == null) throw new ArgumentNullException(nameof(fileName));

        #endregion

        var stream = new FileStream(fileName, FileMode.Create,
            FileAccess.Write, FileShare.None);
        using (stream)
        {
            Save(stream);
        }
    }

    /// <summary>
    ///     Saves the Sequence as a Stream.
    /// </summary>
    /// <param name="stream">
    ///     The stream to use for saving the sequence.
    /// </param>
    public void Save(Stream stream)
    {
        properties.Write(stream);

        var writer = new TrackWriter();

        foreach (var trk in tracks)
        {
            writer.Track = trk;
            writer.Write(stream);
        }
    }

    public void SaveAsync(string fileName)
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        if (IsBusy) throw new InvalidOperationException();

        if (fileName == null) throw new ArgumentNullException(nameof(fileName));

        #endregion

        saveWorker.RunWorkerAsync(fileName);
    }

    public void SaveAsyncCancel()
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        #endregion

        saveWorker.CancelAsync();
    }

    /// <summary>
    ///     Gets the length in ticks of the Sequence.
    /// </summary>
    /// <returns>
    ///     The length in ticks of the Sequence.
    /// </returns>
    /// <remarks>
    ///     The length in ticks of the Sequence is represented by the Track
    ///     with the longest length.
    /// </remarks>
    public int GetLength()
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        #endregion

        return this.Select(t => t.Length).Prepend(0).Max();
    }

    private void OnLoadCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        var handler = LoadCompleted;

        handler?.Invoke(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
    }

    private void OnLoadProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        var handler = LoadProgressChanged;

        handler?.Invoke(this, e);
    }

    private void LoadDoWork(object sender, DoWorkEventArgs e)
    {
        var fileName = (string)e.Argument;

        var stream = new FileStream(fileName, FileMode.Open,
            FileAccess.Read, FileShare.Read);

        using (stream)
        {
            var newProperties = new MidiFileProperties();
            var reader = new TrackReader();
            var newTracks = new List<Track>();

            newProperties.Read(stream);

            for (var i = 0; i < newProperties.TrackCount && !loadWorker.CancellationPending; i++)
            {
                reader.Read(stream);
                newTracks.Add(reader.Track);

                var percentage = (i + 1f) / newProperties.TrackCount;

                loadWorker.ReportProgress((int)(100 * percentage));
            }

            if (loadWorker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                properties = newProperties;
                tracks = newTracks;
            }
        }
    }

    private void OnSaveCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        var handler = SaveCompleted;

        handler?.Invoke(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
    }

    private void OnSaveProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        var handler = SaveProgressChanged;

        handler?.Invoke(this, e);
    }

    private void SaveDoWork(object sender, DoWorkEventArgs e)
    {
        var fileName = (string)e.Argument;

        var stream = new FileStream(fileName, FileMode.Create,
            FileAccess.Write, FileShare.None);

        using (stream)
        {
            properties.Write(stream);

            var writer = new TrackWriter();

            for (var i = 0; i < tracks.Count && !saveWorker.CancellationPending; i++)
            {
                writer.Track = tracks[i];
                writer.Write(stream);

                var percentage = (i + 1f) / properties.TrackCount;

                saveWorker.ReportProgress((int)(100 * percentage));
            }

            if (saveWorker.CancellationPending) e.Cancel = true;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Gets the Track at the specified index.
    /// </summary>
    /// <param name="index">
    ///     The index of the Track to get.
    /// </param>
    /// <returns>
    ///     The Track at the specified index.
    /// </returns>
    public Track this[int index]
    {
        get
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("Sequence");

            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    "Sequence index out of range.");

            #endregion

            return tracks[index];
        }
    }

    /// <summary>
    ///     Gets the Sequence's division value.
    /// </summary>
    public int Division
    {
        get
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("Sequence");

            #endregion

            return properties.Division;
        }
    }

    /// <summary>
    ///     Gets or sets the Sequence's format value.
    /// </summary>
    public int Format
    {
        get
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("Sequence");

            #endregion

            return properties.Format;
        }
        set
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("Sequence");

            if (IsBusy) throw new InvalidOperationException();

            #endregion

            properties.Format = value;
        }
    }

    /// <summary>
    ///     Gets the Sequence's type.
    /// </summary>
    public SequenceType SequenceType
    {
        get
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("Sequence");

            #endregion

            return properties.SequenceType;
        }
    }

    public bool IsBusy => loadWorker.IsBusy || saveWorker.IsBusy;

    #endregion

    #endregion

    #region ICollection<Track> Members

    public void Add(Track item)
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        if (item == null) throw new ArgumentNullException(nameof(item));

        #endregion

        tracks.Add(item);

        properties.TrackCount = tracks.Count;
    }

    public void Clear()
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        #endregion

        tracks.Clear();

        properties.TrackCount = tracks.Count;
    }

    public bool Contains(Track item)
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        #endregion

        return tracks.Contains(item);
    }

    public void CopyTo(Track[] array, int arrayIndex)
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        #endregion

        tracks.CopyTo(array, arrayIndex);
    }

    public int Count
    {
        get
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("Sequence");

            #endregion

            return tracks.Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            #region Require

            if (disposed) throw new ObjectDisposedException("Sequence");

            #endregion

            return false;
        }
    }

    public bool Remove(Track item)
    {
        #region Require

        if (disposed) throw new ObjectDisposedException("Sequence");

        #endregion

        var result = tracks.Remove(item);

        if (result) properties.TrackCount = tracks.Count;

        return result;
    }

    #endregion

    #region IComponent Members

    public event EventHandler Disposed;

    public ISite Site { get; set; }

    #endregion
}