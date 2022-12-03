#region

using System.Diagnostics;
using System.IO;
using BardMusicPlayer.Transmogrify.Song.Importers.GuitarPro.Native;
using BardMusicPlayer.Transmogrify.Song.Utilities;
using Melanchall.DryWetMidi.Core;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.GuitarPro
{
    public static class ImportGuitarPro
    {
        private static GPFile gpfile;

        public static MidiFile OpenGTPSongFile(string path)
        {
            var loader = File.ReadAllBytes(path);
            //Detect Version by Filename
            var version = 7;
            var fileEnding = Path.GetExtension(path);
            version = fileEnding switch
            {
                ".gp3" => 3,
                ".gp4" => 4,
                ".gp5" => 5,
                ".gpx" => 6,
                ".gp" => 7,
                _ => version
            };

            switch (version)
            {
                case 3:
                    gpfile = new GP3File(loader);
                    gpfile.readSong();
                    break;
                case 4:
                    gpfile = new GP4File(loader);
                    gpfile.readSong();
                    break;
                case 5:
                    gpfile = new GP5File(loader);
                    gpfile.readSong();

                    break;
                case 6:
                    gpfile = new GP6File(loader);
                    gpfile.readSong();
                    gpfile = gpfile.self; //Replace with transferred GP5 file

                    break;
                /*case 7:
                    string archiveName = url.Substring(8).Replace("%20", " ");
                    byte[] buffer = new byte[8200000];
                    MemoryStream stream = new MemoryStream(buffer);
                    using (var unzip = new Unzip(archiveName))
                    {
                        //Console.WriteLine("Listing files in the archive:");
                        ListFiles(unzip);

                        unzip.Extract("Content/score.gpif", stream);
                        stream.Position = 0;
                        var sr = new StreamReader(stream);
                        string gp7xml = sr.ReadToEnd();

                        gpfile = new GP7File(gp7xml);
                        gpfile.readSong();
                        gpfile = gpfile.self; //Replace with transferred GP5 file

                    }
                    break;*/
                default:
                    Debug.WriteLine("Unknown File Format");
                    break;
            }

            Debug.WriteLine("Done");

            var song = new NativeFormat(gpfile);
            var midi = song.toMidi();
            var data = midi.createBytes();
            var dataArray = data.ToArray();

            var memoryStream = new MemoryStream();
            memoryStream.Write(dataArray, 0, dataArray.Length);
            memoryStream.Position = 0;
            var midiFile = memoryStream.ReadAsMidiFile();
            memoryStream.Dispose();
            return midiFile;
        }
    }
}