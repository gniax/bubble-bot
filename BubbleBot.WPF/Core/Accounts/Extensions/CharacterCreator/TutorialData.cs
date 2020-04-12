using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BubbleBot.Core.Accounts.Extensions.CharacterCreator
{
    public class Characterdata
    {
        public string charactername { get; set; }
        public uint stepnumber { get; set; }
        public int itemindex { get; set; }
    }

    public class AccountData
    {
        public string accountname { get; set; }
        public List<Characterdata> charactersdata { get; set; }
    }

    public class TutorialData
    {
        public List<AccountData> accounts { get; set; }
    }

    public class TutorialDataMethod
    {
        // Note: Cette fonction va actualiser un personnage qui est dans le tutoriel
        // Si le personnage n'est pas dans le fichier, on l'ajoute, si le compte ne l'est pas, on l'ajoute également
        // Et si le fichier est inexistant, on le crée
        public static void UpdateCharacterData(string character, uint stepnbr, int itemidx, string givenaccountname, string path)
        {
            try
            {
                Characterdata newCharacter = new Characterdata() { charactername = character, stepnumber = stepnbr, itemindex = itemidx };
                List<Characterdata> newListCharacter = new List<Characterdata>();
                newListCharacter.Add(newCharacter);
                AccountData newAcc = new AccountData() { accountname = givenaccountname, charactersdata = newListCharacter }; //nouveau compte

                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    TutorialData FileList = JsonConvert.DeserializeObject<TutorialData>(json);
                    List<AccountData> FileListAccount = FileList.accounts;
                    foreach (AccountData account in FileListAccount)
                    {
                        if (account.accountname == givenaccountname)
                        {
                            List<Characterdata> accountCharacters = account.charactersdata;
                            foreach (Characterdata character1 in accountCharacters)
                            {
                                if (character1.charactername == character)
                                {
                                    character1.stepnumber = stepnbr;
                                    character1.itemindex = itemidx;
                                    string newFile = Newtonsoft.Json.JsonConvert.SerializeObject(FileList, Formatting.Indented);
                                    File.WriteAllText(path, newFile);
                                    return;
                                }
                            }
                            // Si il n'y a pas le personnage on l'ajoute
                            accountCharacters.Add(newCharacter);
                            string newFile1 = Newtonsoft.Json.JsonConvert.SerializeObject(FileList, Formatting.Indented);
                            File.WriteAllText(path, newFile1);
                            return;
                        }
                    }
                    // Si il n'y a pas de comptes correspondant, on l'ajoute avec le personnage
                    FileListAccount.Add(newAcc);
                    string newFile2 = Newtonsoft.Json.JsonConvert.SerializeObject(FileList, Formatting.Indented);
                    File.WriteAllText(path, newFile2);
                    return;
                }
                else // Si le fichier n'existe pas
                {
                    List<AccountData> characterToList = new List<AccountData>();
                    characterToList.Add(newAcc);
                    TutorialData newTutorial = new TutorialData() { accounts = characterToList };
                    using (StreamWriter file = File.CreateText(path))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        //serialize object directly into file stream
                        serializer.Formatting = Formatting.Indented;
                        serializer.Serialize(file, newTutorial);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur - fichier sûrement non existant \n Sinon : {0}", ex);
            }
        }

        // Note: Cette fonction va supprimer un personnage du fichier données du tutoriel
        // Elle ne fonctionnera que si le personnage est présent et le fichier existant évidemment
        public static void RemoveCharacterData(string character, string givenaccountname, string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    TutorialData FileList = JsonConvert.DeserializeObject<TutorialData>(json);
                    List<AccountData> FileListAccount = FileList.accounts;
                    foreach (AccountData account in FileListAccount)
                    {
                        if (account.accountname == givenaccountname)
                        {
                            List<Characterdata> accountCharacters = account.charactersdata;
                            foreach (Characterdata character1 in accountCharacters)
                            {
                                if (character1.charactername == character)
                                {
                                    accountCharacters.Remove(character1);
                                    string newFile = Newtonsoft.Json.JsonConvert.SerializeObject(FileList, Formatting.Indented);
                                    File.WriteAllText(path, newFile);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur - fichier sûrement non existant \n Sinon : {0}", ex);
            }
        }

        // Note: Cette fonction va crée un fichier contenant la classe TutorialData servant à:
        // inscrire l'étape du personnage dans un fichier pour éviter les bugs et pour reprendre le tutoriel à tout moment
        // on supprimera ces données à la fin de la quête.
        public static void StoreFirstCharacterData(AccountData character, string givenaccountname, string path)
        {
            AccountData characterToAdd = character;
            // Soit le fichier existe déjà et donc on va ajouté un compte dedans, sinon on le crée et on ajoute directement le compte.
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                TutorialData FileList = JsonConvert.DeserializeObject<TutorialData>(json);

                // Pour chaque compte situé dans la liste, on vérifie si le compte n'existe pas déjà pour ajouté le compte
                string characterToAddAccount = characterToAdd.accountname;
                foreach (AccountData compte in FileList.accounts)
                {
                    List<Characterdata> listePersos = compte.charactersdata;
                    // Si le compte existe déjà on ajoute juste le personnage dans le compte, sinon on ajoute le compte + perso
                    if (compte.accountname == characterToAddAccount)
                    {
                        Characterdata persoaAdd = characterToAdd.charactersdata.First();
                        listePersos.Add(persoaAdd);
                        string newCharacters = Newtonsoft.Json.JsonConvert.SerializeObject(FileList, Formatting.Indented);
                        File.WriteAllText(path, newCharacters);
                        return;
                    }
                }
                FileList.accounts.Add(characterToAdd);
                string newCharacters1 = Newtonsoft.Json.JsonConvert.SerializeObject(FileList, Formatting.Indented);
                File.WriteAllText(path, newCharacters1);
                return;
            }
            else // Création du fichier s'il n'existe pas ou l'on y ajoute le compte + personnage nouvellement crée
            {
                List<AccountData> characterToList = new List<AccountData>();
                characterToList.Add(characterToAdd);
                TutorialData newCharacter = new TutorialData() { accounts = characterToList };
                using (StreamWriter file = File.CreateText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(file, newCharacter);
                }
            }
        }
        public static uint GetStepNumberFromCharacter(string character, string givenaccountname, string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    TutorialData FileList = JsonConvert.DeserializeObject<TutorialData>(json);
                    List<AccountData> FileListAccount = FileList.accounts;
                    foreach (AccountData account in FileListAccount)
                    {
                        if (account.accountname == givenaccountname)
                        {
                            List<Characterdata> accountCharacters = account.charactersdata;
                            foreach (Characterdata character1 in accountCharacters)
                            {
                                if (character1.charactername == character)
                                {
                                    return character1.stepnumber;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur - fichier sûrement non existant \n Sinon : {0}", ex);
            }
            return 0;
        }

        public static int GetItemIndexFromCharacter(string character, string givenaccountname, string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    TutorialData FileList = JsonConvert.DeserializeObject<TutorialData>(json);
                    List<AccountData> FileListAccount = FileList.accounts;
                    foreach (AccountData account in FileListAccount)
                    {
                        if (account.accountname == givenaccountname)
                        {
                            List<Characterdata> accountCharacters = account.charactersdata;
                            foreach (Characterdata character1 in accountCharacters)
                            {
                                if (character1.charactername == character)
                                {
                                    return character1.itemindex;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur - fichier sûrement non existant \n Sinon : {0}", ex);
            }
            return 0;
        }
    }
}
