﻿using System;

namespace ServiceStackDemo
{
	public class User
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}

