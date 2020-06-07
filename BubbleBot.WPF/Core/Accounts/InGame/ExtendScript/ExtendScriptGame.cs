using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.InGame.ExtendScript
{
    public class ExtendScriptGame : IClearable, IDisposable
    {
        private const string configurationsPath = @"Parameters\ExtendScript\";
        private const string configurationsFolder = @"Parameters\ExtendScript";
        private const string FileExtension = ".EScript";

        // Fields
        private Account _account;
        public string FilePath = "";

        // Constructor
        public ExtendScriptGame(Account account)
        {
            _account = account;
        }

        public void Clear()
        {
        }

        public async Task<bool> CreateFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            // Ensure that the configuration directory is there
            Directory.CreateDirectory(configurationsFolder);

            if (File.Exists(configurationsPath + filename + FileExtension)) return false;

            File.Create(configurationsPath + filename + FileExtension);
            await Task.Delay(500);
            return true;
        }

        public async Task<bool> DeleteFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            // Ensure that the configuration directory is there
            Directory.CreateDirectory(configurationsFolder);

            if (File.Exists(configurationsPath + filename + FileExtension))
            {
                File.Delete(configurationsPath + filename + FileExtension);
                await Task.Delay(500);
                return true;
            }
            return false;
        }

        public async Task<bool> EditValueInt(string filename, string name, int value)
        {
            if (string.IsNullOrEmpty(filename))
                return false;
            var nbtry = 0;
            var maxNbtry = 200;

            while (true)
            {
                try
                {
                    if (File.Exists(configurationsPath + filename + FileExtension))
                    {
                        //Ditcionary contenant toute les variables et valeurs 
                        var AllValueInt = new Dictionary<string, int>();
                        var AllValueString = new Dictionary<string, string>();

                        var nbvariablesint = 0;
                        var nbvariablesstring = 0;

                        using (var br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension,
                            FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            //Search the amount of var int
                            try
                            {
                                nbvariablesint = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesint = 0;
                            }

                            //Search the amount of var string
                            try
                            {
                                nbvariablesstring = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesstring = 0;
                            }

                            //for each var , take the value 
                            if (nbvariablesint > 0)
                                //byte c = br.ReadByte();
                                for (var i = 0; i < nbvariablesint; i++)
                                {
                                    var tmpVar = br.ReadString();
                                    var tmpValue = br.ReadInt32();
                                    AllValueInt.Add(tmpVar, tmpValue);
                                }

                            if (nbvariablesstring > 0)
                                //byte c = br.ReadByte();
                                for (var i = 0; i < nbvariablesstring; i++)
                                {
                                    var tmpVar = br.ReadString();
                                    var tmpValue = br.ReadString();
                                    AllValueString.Add(tmpVar, tmpValue);
                                }

                            br.Close();
                        }

                        // If does'nt exist add the variable 
                        if (!AllValueInt.ContainsKey(name))
                        {
                            AllValueInt.Add(name, value);
                            nbvariablesint++;
                        }

                        // _account.Logger.LogError("AI", "Ecriture du fichier.");
                        using (var bw = new BinaryWriter(File.Open(configurationsPath + filename + FileExtension,
                            FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                        {
                            bw.Write(nbvariablesint);
                            bw.Write(nbvariablesstring);

                            foreach (var obj in AllValueInt)
                                if (obj.Key == name) //if the variable already exist just change value 
                                {
                                    bw.Write(name);
                                    bw.Write(value);
                                }
                                else
                                {
                                    bw.Write(obj.Key);
                                    bw.Write(obj.Value);
                                }

                            //write the list of var 
                            foreach (var obj in AllValueString)
                            {
                                bw.Write(obj.Key);
                                bw.Write(obj.Value);
                            }

                            bw.Close();
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    await Task.Delay(200);
                    //nbtry++;
                }
            }
        }

        public async Task<bool> EditValueString(string filename, string name, string value)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            var nbtry = 0;
            var maxNbtry = 200;

            while (true)
            {
                try
                {
                    if (File.Exists(configurationsPath + filename + FileExtension))
                    {
                        //Ditcionary contenant toute les variables et valeurs 
                        var AllValueInt = new Dictionary<string, int>();
                        var AllValueString = new Dictionary<string, string>();

                        var nbvariablesint = 0;
                        var nbvariablesstring = 0;

                        using (var br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension,
                            FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            //Search the amount of var
                            try
                            {
                                nbvariablesint = br.ReadInt32();
                                nbvariablesstring = br.ReadInt32();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception ex: {0}", ex.Message);
                                nbvariablesint = 0;
                                nbvariablesstring = 0;
                            }

                            //for each var , take the value 
                            if (nbvariablesint > 0)
                                //byte c = br.ReadByte();
                                for (var i = 0; i < nbvariablesint; i++)
                                {
                                    var tmpVar = br.ReadString();
                                    var tmpValue = br.ReadInt32();
                                    AllValueInt.Add(tmpVar, tmpValue);
                                }

                            if (nbvariablesstring > 0)
                                //byte c = br.ReadByte();
                                for (var i = 0; i < nbvariablesstring; i++)
                                {
                                    var tmpVar = br.ReadString();
                                    var tmpValue = br.ReadString();
                                    AllValueString.Add(tmpVar, tmpValue);
                                }

                            br.Close();
                        }

                        // If does'nt exist add the variable 
                        if (!AllValueString.ContainsKey(name))
                        {
                            AllValueString.Add(name, value);
                            nbvariablesstring++;
                        }

                        // _account.Logger.LogError("AI", "Ecriture du fichier.");
                        using (var bw = new BinaryWriter(File.Open(configurationsPath + filename + FileExtension,
                            FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                        {
                            bw.Write(nbvariablesint);
                            bw.Write(nbvariablesstring);

                            foreach (var obj in AllValueInt)
                            {
                                bw.Write(obj.Key);
                                bw.Write(obj.Value);
                            }

                            //write the list of var 
                            foreach (var obj in AllValueString)
                                if (obj.Key == name) //if the variable already exist just change value 
                                {
                                    bw.Write(name);
                                    bw.Write(value);
                                }
                                else
                                {
                                    bw.Write(obj.Key);
                                    bw.Write(obj.Value);
                                }

                            bw.Close();
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    await Task.Delay(200);
                   // nbtry++;
                }
            }
        }

        public async Task<bool> DeleteVariable(string filename, string name)
        {
            if (string.IsNullOrEmpty(filename))
                return false;
            var nbtry = 0;
            var maxNbtry = 200;

            while (nbtry < maxNbtry)
                try
                {
                    if (File.Exists(configurationsPath + filename + FileExtension))
                    {
                        //Ditcionary contenant toute les variables et valeurs 
                        var AllValueInt = new Dictionary<string, int>();
                        var AllValueString = new Dictionary<string, string>();

                        var nbvariablesint = 0;
                        var nbvariablesstring = 0;

                        using (var br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension,
                            FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            //Search the amount of var
                            try
                            {
                                nbvariablesint = br.ReadInt32();
                                nbvariablesstring = br.ReadInt32();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception ex: {0}", ex.Message);
                                nbvariablesint = 0;
                                nbvariablesstring = 0;
                            }

                            //for each var , take the value 
                            if (nbvariablesint > 0)
                                //byte c = br.ReadByte();
                                for (var i = 0; i < nbvariablesint; i++)
                                {
                                    var tmpVar = br.ReadString();
                                    var tmpValue = br.ReadInt32();
                                    if (tmpVar != name)
                                        AllValueInt.Add(tmpVar, tmpValue);
                                    else
                                        nbvariablesint--;
                                }

                            if (nbvariablesstring > 0)
                                //byte c = br.ReadByte();
                                for (var i = 0; i < nbvariablesstring; i++)
                                {
                                    var tmpVar = br.ReadString();
                                    var tmpValue = br.ReadString();
                                    if (tmpVar != name)
                                        AllValueString.Add(tmpVar, tmpValue);
                                    else
                                        nbvariablesstring--;
                                }

                            br.Close();
                        }

                        // _account.Logger.LogError("AI", "Ecriture du fichier.");
                        using (var bw = new BinaryWriter(File.Open(configurationsPath + filename + FileExtension,
                            FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                        {
                            bw.Write(nbvariablesint);
                            bw.Write(nbvariablesstring);

                            foreach (var obj in AllValueInt)
                            {
                                bw.Write(obj.Key);
                                bw.Write(obj.Value);
                            }

                            //write the list of var 
                            foreach (var obj in AllValueString)
                            {
                                bw.Write(obj.Key);
                                bw.Write(obj.Value);
                            }

                            bw.Close();
                        }

                        return true;
                    }
                }
                catch
                {
                    await Task.Delay(200);
                    nbtry++;
                }

            return true;
        }

        public async Task<int> GetValueInt(string filename, string name)
        {

            if (string.IsNullOrEmpty(filename))
                return 0;

            var nbtry = 0;
            var maxNbtry = 200;

            while (true)
            {
                try
                {
                    if (File.Exists(configurationsPath + filename + FileExtension))
                    {
                        var nbvariablesint = 0;
                        var nbvariablesstring = 0;

                        using (var br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension,
                            FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            //Get nb int var 
                            try
                            {
                                nbvariablesint = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesint = 0;
                            }

                            //Get nb string var 
                            try
                            {
                                nbvariablesstring = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesstring = 0;
                            }

                            if (nbvariablesint > 0)
                                for (var i = 0; i < nbvariablesint; i++)
                                {
                                    var tmpvalname = br.ReadString();
                                    var tmpval = br.ReadInt32();
                                    if (tmpvalname == name)
                                    {
                                        br.Close();
                                        return tmpval;
                                    }
                                }

                            br.Close();
                        }
                        return 0;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                   await Task.Delay(200);
                   //nbtry++;
                }
            }
          //  return 0;
        }

        public async Task<string> GetValueString(string filename, string name)
        {
            if (string.IsNullOrEmpty(filename))
                return "";
           // var nbtry = 0;
            //var maxNbtry = 200;

            while (true)
            {
                try
                {
                    if (File.Exists(configurationsPath + filename + FileExtension))
                    {
                        var nbvariablesint = 0;
                        var nbvariablesstring = 0;

                        using (var br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension,
                            FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            //Get nb int var 
                            try
                            {
                                nbvariablesint = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesint = 0;
                            }

                            //Get nb string var 
                            try
                            {
                                nbvariablesstring = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesstring = 0;
                            }

                            if (nbvariablesint > 0)
                                for (var i = 0; i < nbvariablesint; i++)
                                {
                                    br.ReadString();
                                    br.ReadInt32();
                                }

                            if (nbvariablesstring > 0)
                                for (var i = 0; i < nbvariablesstring; i++)
                                {
                                    var tmpvalname = br.ReadString();
                                    var tmpval = br.ReadString();
                                    if (tmpvalname == name)
                                    {
                                        br.Close();
                                        return tmpval;
                                    }
                                }

                            br.Close();
                        }

                        return "";
                    }
                    else
                    {
                        return "";
                    }
                }
                catch
                {
                    await Task.Delay(200);
                   // nbtry++;
                }
            }
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                _account = null;
                // _FileSemaphore = null;

                disposedValue = true;
            }
        }

        ~ExtendScriptGame()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}