using Windows.Storage;

namespace HIU.Core.Common
{
	public class SettingsManager
	{
		public static void Set<T>(T settingsObject, string key) where T : class
		{
			ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
			roamingSettings.Values[key] = settingsObject;
		}

		public static void SetSerialized<T>(T settingsObject, string key) where T : class
		{
			ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
			roamingSettings.Values[key] = JsonManager.Serialize<T>(settingsObject);
		}

		public static T Get<T>(string key) where T : class
		{
			ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
			T settingsValue = default(T);

			if (roamingSettings.Values.ContainsKey(key))
			{
				settingsValue = roamingSettings.Values[key] as T;
			}

			return settingsValue;
		}

		public static T GetSerialized<T>(string key) where T : class
		{
			ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
			T settingsValue = default(T);

			if (roamingSettings.Values.ContainsKey(key))
			{
				settingsValue = JsonManager.Deserialize<T>(roamingSettings.Values[key].ToString());
			}

			return settingsValue;
		}

		public static void Remove(string key)
		{
			ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
			if (roamingSettings.Values.ContainsKey(key))
			{
				roamingSettings.Values.Remove(key);
			}

		}

		public static bool Exist(string key)
		{
			ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
			return roamingSettings.Values.ContainsKey(key);
		}
	}
}
