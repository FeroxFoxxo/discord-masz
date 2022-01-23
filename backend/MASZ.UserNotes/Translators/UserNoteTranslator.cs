﻿using MASZ.Bot.Abstractions;
using MASZ.Bot.Enums;

namespace MASZ.UserNotes.Translators;

public class UserNoteTranslator : Translator
{
	public string UserNote()
	{
		return PreferredLanguage switch
		{
			Language.De => "Benutzernotiz",
			Language.At => "Benutzanotiz",
			Language.Fr => "Note de l'utilisateur",
			Language.Es => "UserNote",
			Language.Ru => "UserNote",
			Language.It => "Nota utente",
			_ => "UserNote"
		};
	}
	
	public string UserNoteId()
	{
		return PreferredLanguage switch
		{
			Language.De => "Benutzernotiz-ID",
			Language.At => "NutzaNotizID",
			Language.Fr => "Identifiant de note d'utilisateur",
			Language.Es => "ID de nota de usuario",
			Language.Ru => "идентификатор пользовательской заметки",
			Language.It => "ID nota utente",
			_ => "User Note ID"
		};
	}
}