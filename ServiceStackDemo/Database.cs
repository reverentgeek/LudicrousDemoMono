using System;
using System.Collections.Generic;
using System.Linq;
using Biggy;

namespace ServiceStackDemo
{
	public static class Database
	{
		static BiggyList<User> _users;

		public static void Load(string path)
		{
			_users = new BiggyList<User> (dbPath: path);
		}

		public static BiggyList<User> Users 
		{
			get { return _users; }
		}
	}
}

