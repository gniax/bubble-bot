using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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
        public static void UpdateCharacterData(string character, uint stepnbr, int itemidx, string givenaccountname,
            string path)
        {
            try
            {
                var newCharacter = new Characterdata
                    {charactername = character, stepnumber = stepnbr, itemindex = itemidx};
                var newListCharacter = new List<Characterdata>();
                newListCharacter.Add(newCharacter);
                var newAcc = new AccountData
                    {accountname = givenaccountname, charactersdata = newListCharacter}; //nouveau compte

                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    var FileList = JsonConvert.DeserializeObject<TutorialData>(json);
                    var FileListAccount = FileList.accounts;
                    foreach (var account in FileListAccount)
                        if (account.accountname == givenaccountname)
                        {
                            var accountCharacters = account.charactersdata;
                            foreach (var character1 in accountCharacters)
                                if (character1.charactername == character)
                                {
                                    character1.stepnumber = stepnbr;
                                    character1.itemindex = itemidx;
                                    var newFile = JsonConvert.SerializeObject(FileList, Formatting.Indented);
                                    File.WriteAllText(path, newFile);
                                    return;
                                }

                            // Si il n'y a pas le personnage on l'ajoute
                            accountCharacters.Add(newCharacter);
                            var newFile1 = JsonConvert.SerializeObject(FileList, Formatting.Indented);
                            File.WriteAllText(path, newFile1);
                            return;
                        }

                    // Si il n'y a pas de comptes correspondant, on l'ajoute avec le personnage
                    FileListAccount.Add(newAcc);
                    var newFile2 = JsonConvert.SerializeObject(FileList, Formatting.Indented);
                    File.WriteAllText(path, newFile2);
                }
                else // Si le fichier n'existe pas
                {
                    var characterToList = new List<AccountData>();
                    characterToList.Add(newAcc);
                    var newTutorial = new TutorialData {accounts = characterToList};
                    using (var file = File.CreateText(path))
                    {
                        var serializer = new JsonSerializer();
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
                    var json = File.ReadAllText(path);
                    var FileList = JsonConvert.DeserializeObject<TutorialData>(json);
                    var FileListAccount = FileList.accounts;
                    foreach (var account in FileListAccount)
                        if (account.accountname == givenaccountname)
                        {
                            var accountCharacters = account.charactersdata;
                            foreach (var character1 in accountCharacters)
                                if (character1.charactername == character)
                                {
                                    accountCharacters.Remove(character1);
                                    var newFile = JsonConvert.SerializeObject(FileList, Formatting.Indented);
                                    File.WriteAllText(path, newFile);
                                    return;
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
            var characterToAdd = character;
            // Soit le fichier existe déjà et donc on va ajouté un compte dedans, sinon on le crée et on ajoute directement le compte.
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var FileList = JsonConvert.DeserializeObject<TutorialData>(json);

                // Pour chaque compte situé dans la liste, on vérifie si le compte n'existe pas déjà pour ajouté le compte
                var characterToAddAccount = characterToAdd.accountname;
                foreach (var compte in FileList.accounts)
                {
                    var listePersos = compte.charactersdata;
                    // Si le compte existe déjà on ajoute juste le personnage dans le compte, sinon on ajoute le compte + perso
                    if (compte.accountname == characterToAddAccount)
                    {
                        var persoaAdd = characterToAdd.charactersdata.First();
                        listePersos.Add(persoaAdd);
                        var newCharacters = JsonConvert.SerializeObject(FileList, Formatting.Indented);
                        File.WriteAllText(path, newCharacters);
                        return;
                    }
                }

                FileList.accounts.Add(characterToAdd);
                var newCharacters1 = JsonConvert.SerializeObject(FileList, Formatting.Indented);
                File.WriteAllText(path, newCharacters1);
            }
            else // Création du fichier s'il n'existe pas ou l'on y ajoute le compte + personnage nouvellement crée
            {
                var characterToList = new List<AccountData>();
                characterToList.Add(characterToAdd);
                var newCharacter = new TutorialData {accounts = characterToList};
                using (var file = File.CreateText(path))
                {
                    var serializer = new JsonSerializer();
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
                    var json = File.ReadAllText(path);
                    var FileList = JsonConvert.DeserializeObject<TutorialData>(json);
                    var FileListAccount = FileList.accounts;
                    foreach (var account in FileListAccount)
                        if (account.accountname == givenaccountname)
                        {
                            var accountCharacters = account.charactersdata;
                            foreach (var character1 in accountCharacters)
                                if (character1.charactername == character)
                                    return character1.stepnumber;
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
                    var json = File.ReadAllText(path);
                    var FileList = JsonConvert.DeserializeObject<TutorialData>(json);
                    var FileListAccount = FileList.accounts;
                    foreach (var account in FileListAccount)
                        if (account.accountname == givenaccountname)
                        {
                            var accountCharacters = account.charactersdata;
                            foreach (var character1 in accountCharacters)
                                if (character1.charactername == character)
                                    return character1.itemindex;
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