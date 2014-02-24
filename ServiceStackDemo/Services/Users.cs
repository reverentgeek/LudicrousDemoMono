using System;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using ServiceStack;

namespace ServiceStackDemo
{

	[Route("/user", "GET")]
	public class UserListRequest : IReturn<List<User>>
	{
		public UserListRequest() 
		{
			PageNumber = 1;
			PageSize = 10;
		}

		public int PageNumber { get; set; }
		public int PageSize { get; set; }
	}

	[Route("/user/{Id}", "GET")]
	public class UserGetRequest : IReturn<User>
	{
		public Guid Id { get; set; }
	}

	[Route("/user", "POST")]
	[Route("/user/{Id}", "PUT")]
	public class UserCreateUpdateRequest : IReturn<User>
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
	}
		
	[Route("/user/{Id}", "DELETE")]
	public class UserDeleteRequest : IReturn<bool>
	{
		public Guid Id { get; set; }
	}

	[Route("/user/init", "GET")]
	public class UserInitRequest : IReturn<bool>
	{}

	public class Users: Service
	{
		private UserRepository _userRepository;

		public Users() { _userRepository = new UserRepository(); }

		public object Get(UserListRequest request)
		{
			var users = _userRepository.GetUsers (request.PageNumber, request.PageSize);

			return users;
		}

		public object Get(UserGetRequest request)
		{
			var user = _userRepository.GetUser(request.Id);
			if (user != null)
				return user;
			throw HttpError.NotFound("User not found.");
		}

		public object Post(UserCreateUpdateRequest request) 
		{
			if (string.IsNullOrWhiteSpace(request.FirstName) 
				|| string.IsNullOrWhiteSpace(request.LastName) 
				|| string.IsNullOrWhiteSpace(request.EmailAddress))
				throw new ArgumentException ("FirstName, LastName, and EmailAddress are required.");

			var user = new User {
				Id = Guid.NewGuid(),
				FirstName = request.FirstName,
				LastName = request.LastName,
				EmailAddress = request.EmailAddress,
				CreatedDate = DateTime.UtcNow
			};

			return _userRepository.AddUser(user);
		}

		public object Put(UserCreateUpdateRequest request)
		{
			if (request.Id == Guid.Empty
				|| string.IsNullOrWhiteSpace(request.FirstName) 
				|| string.IsNullOrWhiteSpace(request.LastName) 
				|| string.IsNullOrWhiteSpace(request.EmailAddress))
				throw new ArgumentException ("Id, FirstName, LastName, and EmailAddress are required.");

			var user = new User {
				Id = request.Id,
				FirstName = request.FirstName,
				LastName = request.LastName,
				EmailAddress = request.EmailAddress
			};

			var updatedUser = _userRepository.UpdateUser(user);

			if (updatedUser == null)
				throw HttpError.NotFound("User not found.");

			return updatedUser;
		}

		public object Delete(UserDeleteRequest request)
		{
			_userRepository.DeleteUser(request.Id);
			return true;
		}

		public object Get(UserInitRequest request)
		{
			_userRepository.InitDemoUsers();
			return true;
		}
	}

	public class UserRepository
	{
		public List<User> GetUsers(int pageNumber, int pageSize)
		{
			return Database.Users
					.OrderByDescending(x => x.CreatedDate)
					.Skip((pageNumber - 1) * pageSize)
					.Take(pageSize)
					.ToList();
		}

		public User GetUser(Guid id)
		{
			return Database.Users.FirstOrDefault(x => x.Id == id);
		}

		public User AddUser(User user)
		{
			if (user == null)
				throw new Exception("There was a problem with your request. Please check your data and try again.");
			Database.Users.Add(user);
			Database.Users.Save();
			return user;
		}

		public User UpdateUser(User user)
		{
			if (user == null)
				throw new Exception("There was a problem with your request. Please check your data and try again.");

			var existingUser = Database.Users.Where (x => x.Id == user.Id).FirstOrDefault();

			if (existingUser == null)
				return null;

			existingUser.FirstName = user.FirstName;
			existingUser.LastName = user.LastName;
			existingUser.EmailAddress = user.EmailAddress;

			Database.Users.Update(existingUser);
			Database.Users.Save();
			return existingUser;

		}

		public void DeleteUser(Guid id)
		{
			var user = Database.Users.Where(x => x.Id == id).First();
			Database.Users.Remove(user);
			Database.Users.Save();
		}

		public void InitDemoUsers()
		{
			Database.Users.ClearAndSave();

			var demoUsers = new List<User> {
				new User {
					Id = Guid.NewGuid(), FirstName = "Howard",
					LastName = "Cummings",
					EmailAddress = "id.enim@Curabituregestas.org",
					CreatedDate = DateTime.Parse("10/03/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Azalia",
					LastName = "Hubbard",
					EmailAddress = "ridiculus.mus.Proin@nullaatsem.edu",
					CreatedDate = DateTime.Parse("07/30/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Nita",
					LastName = "Johnston",
					EmailAddress = "Nulla.aliquet@enimEtiam.com",
					CreatedDate = DateTime.Parse("10/18/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Laith",
					LastName = "Mckenzie",
					EmailAddress = "sit.amet.risus@blanditmattisCras.edu",
					CreatedDate = DateTime.Parse("04/26/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Daryl",
					LastName = "Nguyen",
					EmailAddress = "sed@sapienCras.co.uk",
					CreatedDate = DateTime.Parse("11/29/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Ulric",
					LastName = "Estes",
					EmailAddress = "id.risus@diamdictumsapien.co.uk",
					CreatedDate = DateTime.Parse("08/09/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Amelia",
					LastName = "Yates",
					EmailAddress = "lectus.quis.massa@idlibero.com",
					CreatedDate = DateTime.Parse("03/08/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Shafira",
					LastName = "Frank",
					EmailAddress = "natoque.penatibus@senectusetnetus.edu",
					CreatedDate = DateTime.Parse("03/20/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Ross",
					LastName = "Mays",
					EmailAddress = "sagittis.semper@urnajusto.edu",
					CreatedDate = DateTime.Parse("11/08/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Felicia",
					LastName = "Young",
					EmailAddress = "Cras@Maecenasiaculis.ca",
					CreatedDate = DateTime.Parse("03/08/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Orla",
					LastName = "Lowery",
					EmailAddress = "rutrum.justo.Praesent@necmetus.org",
					CreatedDate = DateTime.Parse("08/12/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Hadley",
					LastName = "Kirby",
					EmailAddress = "sit.amet.ultricies@Donecnibh.net",
					CreatedDate = DateTime.Parse("11/10/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Ryan",
					LastName = "Benton",
					EmailAddress = "Quisque.purus@volutpatNulladignissim.co.uk",
					CreatedDate = DateTime.Parse("07/21/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Hillary",
					LastName = "Roy",
					EmailAddress = "enim.diam@Morbiaccumsan.org",
					CreatedDate = DateTime.Parse("06/20/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Beck",
					LastName = "Cote",
					EmailAddress = "ut@luctuslobortisClass.ca",
					CreatedDate = DateTime.Parse("12/22/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Matthew",
					LastName = "Slater",
					EmailAddress = "Cum.sociis@ante.net",
					CreatedDate = DateTime.Parse("11/09/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Miriam",
					LastName = "Moore",
					EmailAddress = "porttitor.eros.nec@ornare.ca",
					CreatedDate = DateTime.Parse("01/22/2014")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Virginia",
					LastName = "Reese",
					EmailAddress = "a.aliquet@faucibusMorbivehicula.edu",
					CreatedDate = DateTime.Parse("04/07/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Graiden",
					LastName = "Holman",
					EmailAddress = "diam@aliquetmetus.com",
					CreatedDate = DateTime.Parse("05/01/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Beverly",
					LastName = "Sosa",
					EmailAddress = "orci.Phasellus.dapibus@Suspendissetristiqueneque.com",
					CreatedDate = DateTime.Parse("12/07/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Nolan",
					LastName = "Perkins",
					EmailAddress = "sodales@at.edu",
					CreatedDate = DateTime.Parse("03/09/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Nicole",
					LastName = "Pena",
					EmailAddress = "non.justo@pede.org",
					CreatedDate = DateTime.Parse("03/04/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Elmo",
					LastName = "Parrish",
					EmailAddress = "Nunc.ac.sem@iderat.ca",
					CreatedDate = DateTime.Parse("11/18/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Ursula",
					LastName = "Whitley",
					EmailAddress = "dolor.dolor.tempus@diam.com",
					CreatedDate = DateTime.Parse("06/04/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Quon",
					LastName = "Cline",
					EmailAddress = "Suspendisse.commodo@elitAliquamauctor.net",
					CreatedDate = DateTime.Parse("04/25/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Josephine",
					LastName = "Dunn",
					EmailAddress = "iaculis.odio@nibh.com",
					CreatedDate = DateTime.Parse("09/07/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Willow",
					LastName = "Guthrie",
					EmailAddress = "ac.mattis@penatibus.co.uk",
					CreatedDate = DateTime.Parse("05/31/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Allistair",
					LastName = "Ward",
					EmailAddress = "risus.In@egestas.ca",
					CreatedDate = DateTime.Parse("04/07/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Harriet",
					LastName = "Foster",
					EmailAddress = "consequat@erat.edu",
					CreatedDate = DateTime.Parse("05/16/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Susan",
					LastName = "Schwartz",
					EmailAddress = "a.malesuada@Cras.net",
					CreatedDate = DateTime.Parse("06/05/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Hyatt",
					LastName = "Michael",
					EmailAddress = "In.lorem@duiCras.edu",
					CreatedDate = DateTime.Parse("06/05/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Sopoline",
					LastName = "Parks",
					EmailAddress = "ridiculus.mus.Donec@tristiqueac.edu",
					CreatedDate = DateTime.Parse("05/19/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Britanney",
					LastName = "Bowman",
					EmailAddress = "ipsum.Phasellus@Utsemperpretium.net",
					CreatedDate = DateTime.Parse("05/23/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Belle",
					LastName = "Rice",
					EmailAddress = "lacus.Quisque.purus@quisurna.net",
					CreatedDate = DateTime.Parse("10/16/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Sean",
					LastName = "Reyes",
					EmailAddress = "non@a.ca",
					CreatedDate = DateTime.Parse("07/07/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Felicia",
					LastName = "Clemons",
					EmailAddress = "turpis.vitae@Nullatempor.net",
					CreatedDate = DateTime.Parse("12/31/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Herman",
					LastName = "Pena",
					EmailAddress = "Curabitur.consequat@a.com",
					CreatedDate = DateTime.Parse("05/15/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Illana",
					LastName = "Keith",
					EmailAddress = "eu.dolor.egestas@facilisismagna.net",
					CreatedDate = DateTime.Parse("07/30/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Zachery",
					LastName = "Mendoza",
					EmailAddress = "Integer.mollis@aclibero.net",
					CreatedDate = DateTime.Parse("06/09/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Bruno",
					LastName = "Oneal",
					EmailAddress = "orci.Phasellus@dolorNulla.ca",
					CreatedDate = DateTime.Parse("11/03/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Vernon",
					LastName = "Fitzgerald",
					EmailAddress = "eu.arcu@pharetranibhAliquam.ca",
					CreatedDate = DateTime.Parse("06/08/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Dorian",
					LastName = "Atkins",
					EmailAddress = "nisi.sem@pharetra.com",
					CreatedDate = DateTime.Parse("06/26/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Donovan",
					LastName = "Winters",
					EmailAddress = "dapibus.gravida.Aliquam@sociosquadlitora.net",
					CreatedDate = DateTime.Parse("07/06/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Francis",
					LastName = "Maxwell",
					EmailAddress = "tincidunt@Aliquamnecenim.org",
					CreatedDate = DateTime.Parse("07/08/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Ignatius",
					LastName = "Peterson",
					EmailAddress = "turpis.egestas@bibendumDonecfelis.edu",
					CreatedDate = DateTime.Parse("03/14/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Gregory",
					LastName = "Ballard",
					EmailAddress = "odio.Etiam@nibh.co.uk",
					CreatedDate = DateTime.Parse("08/21/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Yetta",
					LastName = "Burnett",
					EmailAddress = "a.neque@ut.edu",
					CreatedDate = DateTime.Parse("07/29/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Cameran",
					LastName = "Chapman",
					EmailAddress = "odio.tristique@disparturient.org",
					CreatedDate = DateTime.Parse("02/16/2014")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Olga",
					LastName = "Harrison",
					EmailAddress = "Vivamus@duiquisaccumsan.ca",
					CreatedDate = DateTime.Parse("03/09/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Conan",
					LastName = "Reeves",
					EmailAddress = "diam.at@loremluctusut.co.uk",
					CreatedDate = DateTime.Parse("12/01/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Asher",
					LastName = "Ayers",
					EmailAddress = "fringilla@per.ca",
					CreatedDate = DateTime.Parse("03/07/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Beverly",
					LastName = "Lloyd",
					EmailAddress = "Cras.vulputate.velit@enimEtiamimperdiet.co.uk",
					CreatedDate = DateTime.Parse("03/26/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Shay",
					LastName = "Hendricks",
					EmailAddress = "dolor.nonummy@Vivamus.org",
					CreatedDate = DateTime.Parse("02/27/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Aidan",
					LastName = "Bowen",
					EmailAddress = "vulputate@arcu.com",
					CreatedDate = DateTime.Parse("06/12/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Gary",
					LastName = "Alvarado",
					EmailAddress = "lectus@duinec.com",
					CreatedDate = DateTime.Parse("07/06/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Bell",
					LastName = "Travis",
					EmailAddress = "egestas.a.scelerisque@orciadipiscing.com",
					CreatedDate = DateTime.Parse("05/01/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Kareem",
					LastName = "Dalton",
					EmailAddress = "at@Phaselluslibero.com",
					CreatedDate = DateTime.Parse("11/23/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Alice",
					LastName = "Chase",
					EmailAddress = "fringilla.mi@Vestibulumut.co.uk",
					CreatedDate = DateTime.Parse("04/18/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Troy",
					LastName = "Sullivan",
					EmailAddress = "mi.pede.nonummy@Sedegetlacus.edu",
					CreatedDate = DateTime.Parse("10/12/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Kaseem",
					LastName = "Reese",
					EmailAddress = "netus.et.malesuada@nibh.ca",
					CreatedDate = DateTime.Parse("05/18/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Joshua",
					LastName = "Shelton",
					EmailAddress = "aliquam.eu@ullamcorper.net",
					CreatedDate = DateTime.Parse("01/01/2014")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Kylie",
					LastName = "Bowman",
					EmailAddress = "ad@mollis.net",
					CreatedDate = DateTime.Parse("06/24/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Armand",
					LastName = "Santana",
					EmailAddress = "nulla@dictummagnaUt.ca",
					CreatedDate = DateTime.Parse("05/15/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Stewart",
					LastName = "Mcdaniel",
					EmailAddress = "Vestibulum.accumsan@quamPellentesquehabitant.edu",
					CreatedDate = DateTime.Parse("10/10/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Bevis",
					LastName = "Joyner",
					EmailAddress = "ridiculus.mus@auctor.com",
					CreatedDate = DateTime.Parse("10/13/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Phelan",
					LastName = "Cruz",
					EmailAddress = "pharetra.felis@in.ca",
					CreatedDate = DateTime.Parse("01/28/2014")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Dora",
					LastName = "Randall",
					EmailAddress = "tempus@fermentumarcuVestibulum.com",
					CreatedDate = DateTime.Parse("01/09/2014")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Garrett",
					LastName = "Vang",
					EmailAddress = "et@acorci.com",
					CreatedDate = DateTime.Parse("08/03/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Carter",
					LastName = "Vaughan",
					EmailAddress = "Aliquam.auctor@Aliquam.org",
					CreatedDate = DateTime.Parse("09/14/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Cherokee",
					LastName = "Potter",
					EmailAddress = "in.felis.Nulla@id.co.uk",
					CreatedDate = DateTime.Parse("07/18/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Miranda",
					LastName = "Valentine",
					EmailAddress = "consequat@condimentum.edu",
					CreatedDate = DateTime.Parse("08/27/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Elizabeth",
					LastName = "Carter",
					EmailAddress = "sapien.imperdiet.ornare@scelerisqueneque.org",
					CreatedDate = DateTime.Parse("12/16/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Gregory",
					LastName = "Brooks",
					EmailAddress = "netus.et.malesuada@nondapibus.net",
					CreatedDate = DateTime.Parse("07/31/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Raya",
					LastName = "Frost",
					EmailAddress = "Sed.dictum.Proin@dui.ca",
					CreatedDate = DateTime.Parse("04/01/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Marvin",
					LastName = "Ruiz",
					EmailAddress = "ipsum.cursus@nunc.com",
					CreatedDate = DateTime.Parse("06/06/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Judah",
					LastName = "Johns",
					EmailAddress = "ornare.elit.elit@vulputateposuere.com",
					CreatedDate = DateTime.Parse("01/25/2014")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Illiana",
					LastName = "Gill",
					EmailAddress = "facilisis.vitae@eleifendegestas.co.uk",
					CreatedDate = DateTime.Parse("05/04/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Alma",
					LastName = "Sutton",
					EmailAddress = "lacus.Etiam@molestiedapibus.edu",
					CreatedDate = DateTime.Parse("03/04/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Zahir",
					LastName = "Shelton",
					EmailAddress = "et.netus@id.ca",
					CreatedDate = DateTime.Parse("02/15/2014")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Chancellor",
					LastName = "Kramer",
					EmailAddress = "Nunc.commodo@Phaselluselitpede.org",
					CreatedDate = DateTime.Parse("05/18/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Priscilla",
					LastName = "Buckner",
					EmailAddress = "cursus.diam@ametante.ca",
					CreatedDate = DateTime.Parse("02/04/2014")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Caesar",
					LastName = "Foreman",
					EmailAddress = "non.justo@ullamcorpernisl.org",
					CreatedDate = DateTime.Parse("12/17/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Ebony",
					LastName = "Stewart",
					EmailAddress = "molestie.sodales.Mauris@egestasa.org",
					CreatedDate = DateTime.Parse("12/21/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Anthony",
					LastName = "Hubbard",
					EmailAddress = "lacinia.Sed.congue@necmaurisblandit.org",
					CreatedDate = DateTime.Parse("06/02/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Kathleen",
					LastName = "Lester",
					EmailAddress = "nunc.risus@Vivamusnisi.edu",
					CreatedDate = DateTime.Parse("12/26/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Myles",
					LastName = "Flowers",
					EmailAddress = "Curabitur.massa.Vestibulum@ligulaAenean.co.uk",
					CreatedDate = DateTime.Parse("04/16/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Hadassah",
					LastName = "Christian",
					EmailAddress = "neque.Nullam.nisl@dolor.edu",
					CreatedDate = DateTime.Parse("07/16/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Mark",
					LastName = "Duke",
					EmailAddress = "netus.et.malesuada@ametloremsemper.net",
					CreatedDate = DateTime.Parse("09/04/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Cody",
					LastName = "Nash",
					EmailAddress = "Vivamus.nisi@Lorem.org",
					CreatedDate = DateTime.Parse("07/16/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Joshua",
					LastName = "Browning",
					EmailAddress = "justo.faucibus@ametluctus.ca",
					CreatedDate = DateTime.Parse("10/06/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Ross",
					LastName = "Luna",
					EmailAddress = "vel.est@Namporttitorscelerisque.edu",
					CreatedDate = DateTime.Parse("10/19/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Malachi",
					LastName = "Mcleod",
					EmailAddress = "Pellentesque.ultricies@non.edu",
					CreatedDate = DateTime.Parse("09/27/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Eaton",
					LastName = "Langley",
					EmailAddress = "dapibus.id@quamquis.org",
					CreatedDate = DateTime.Parse("02/24/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Francis",
					LastName = "Hooper",
					EmailAddress = "augue@Fuscemilorem.org",
					CreatedDate = DateTime.Parse("08/21/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Moses",
					LastName = "Kane",
					EmailAddress = "tellus.Nunc.lectus@ipsum.org",
					CreatedDate = DateTime.Parse("01/14/2014")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Laurel",
					LastName = "Nelson",
					EmailAddress = "et.eros.Proin@lectus.ca",
					CreatedDate = DateTime.Parse("11/28/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Joseph",
					LastName = "Knox",
					EmailAddress = "fermentum.vel.mauris@lacinia.co.uk",
					CreatedDate = DateTime.Parse("12/13/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Jessamine",
					LastName = "Duke",
					EmailAddress = "tristique.pellentesque@nonvestibulumnec.co.uk",
					CreatedDate = DateTime.Parse("07/12/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Paul",
					LastName = "Mcclure",
					EmailAddress = "Cum@posuereenimnisl.net",
					CreatedDate = DateTime.Parse("07/01/2013")
				},
				new User {
					Id = Guid.NewGuid(), FirstName = "Ora",
					LastName = "England",
					EmailAddress = "auctor.vitae.aliquet@sitametornare.ca",
					CreatedDate = DateTime.Parse("05/01/2013")
				}
			};

			foreach (var user in demoUsers)
				Database.Users.Add(user);

			Database.Users.Save();
		}
	}
}

