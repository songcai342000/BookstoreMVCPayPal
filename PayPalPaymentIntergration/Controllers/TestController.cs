using Microsoft.AspNetCore.Mvc;
using PayPalPaymentIntergration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalPaymentIntergration.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        protected static List<Student> students = new List<Student>
    {
        new Student {FirstName = "Terry", LastName = "Adams", ID = 120,
            Year = GradeLevel.SecondYear,
            ExamScores = new List<int>{ 99, 82, 81, 79}},
        new Student {FirstName = "Fadi", LastName = "Fakhouri", ID = 116,
            Year = GradeLevel.ThirdYear,
            ExamScores = new List<int>{ 99, 86, 90, 94}},
        new Student {FirstName = "Hanying", LastName = "Feng", ID = 117,
            Year = GradeLevel.FirstYear,
            ExamScores = new List<int>{ 93, 92, 80, 87}},
        new Student {FirstName = "Cesar", LastName = "Garcia", ID = 114,
            Year = GradeLevel.FourthYear,
            ExamScores = new List<int>{ 97, 89, 85, 82}},
        new Student {FirstName = "Debra", LastName = "Garcia", ID = 115,
            Year = GradeLevel.ThirdYear,
            ExamScores = new List<int>{ 35, 72, 91, 70}},
        new Student {FirstName = "Hugo", LastName = "Garcia", ID = 118,
            Year = GradeLevel.SecondYear,
            ExamScores = new List<int>{ 92, 90, 83, 78}},
        new Student {FirstName = "Sven", LastName = "Mortensen", ID = 113,
            Year = GradeLevel.FirstYear,
            ExamScores = new List<int>{ 88, 94, 65, 91}},
        new Student {FirstName = "Claire", LastName = "O'Donnell", ID = 112,
            Year = GradeLevel.FourthYear,
            ExamScores = new List<int>{ 75, 84, 91, 39}},
        new Student {FirstName = "Svetlana", LastName = "Omelchenko", ID = 111,
            Year = GradeLevel.SecondYear,
            ExamScores = new List<int>{ 97, 92, 81, 60}},
        new Student {FirstName = "Lance", LastName = "Tucker", ID = 119,
            Year = GradeLevel.ThirdYear,
            ExamScores = new List<int>{ 68, 79, 88, 92}},
        new Student {FirstName = "Michael", LastName = "Tucker", ID = 122,
            Year = GradeLevel.FirstYear,
            ExamScores = new List<int>{ 94, 92, 91, 91}},
        new Student {FirstName = "Eugene", LastName = "Zabokritski", ID = 121,
            Year = GradeLevel.FourthYear,
            ExamScores = new List<int>{ 96, 85, 91, 60}}
    };

        protected static List<SubjectNumber> subjectNumbers = new List<SubjectNumber>()
        {
            new SubjectNumber {ID=1, Number=3},
            new SubjectNumber {ID=2, Number=3},
            new SubjectNumber {ID=3, Number=3},
        };

        //Show SubjectNumbers
        public IActionResult SubjectNumbers()
        {
            return View(subjectNumbers.ToList());
        }

        //Create new SubjectNumber
        public IActionResult CreateSubjectNumber()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSubjectNumber([Bind("ID,Number")] SubjectNumber subjectNumber)
        {
            if (ModelState.IsValid)
            {
                subjectNumbers.Add(subjectNumber);
                return RedirectToAction(nameof(SubjectNumbers));
            }
            return View(subjectNumber);
        }

        [HttpGet]
        public IActionResult TotalSubjectNumber()
        {
            var totalSubjectNumber = subjectNumbers.Select(number => number.Number).Sum();
            return View(totalSubjectNumber);
        }

        //events
        protected static List<Event> events = new List<Event>()
        {
            new Event {Time=new DateTime(2021, 09, 17), EventItem="Consert"},
            new Event {Time=DateTime.Now.AddDays(-3), EventItem="Consert"}
        };

        //Show SubjectNumbers
        public IActionResult Events()
        {
            return View(events.ToList());
        }

        //Create new SubjectNumber
        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEvent([Bind("Event,EventItem")] Event @event)
        {
            if (ModelState.IsValid)
            {
                events.Add(@event);
                return RedirectToAction(nameof(Events));
            }
            return View(@event);
        }

        [HttpGet]
        public IActionResult QueryHighScores(int exam, int score)
        {
            // Variable queryLastNames is an IEnumerable<IGrouping<string,
            // DataClass.Student>>.
            var queryLastNames =
                from student in students
                group student by student.LastName into newGroup
                orderby newGroup.Key
                select newGroup;
            var groupMembers = new List<Student>();
            foreach (var nameGroup in queryLastNames)
            {
                foreach (var student in nameGroup)
                {
                    groupMembers.Add(student);
                }
            }

            return View(groupMembers);
        }

        [HttpGet]
        public IActionResult PetsAndOwners()
        {
            Person magnus = new Person { FirstName = "Magnus", LastName = "Hedlund" };
            Person terry = new Person { FirstName = "Terry", LastName = "Adams" };
            Person charlotte = new Person { FirstName = "Charlotte", LastName = "Weiss" };
            Person arlene = new Person { FirstName = "Arlene", LastName = "Huff" };

            Pet barley = new Pet { Name = "Barley", Owner = terry };
            Pet boots = new Pet { Name = "Boots", Owner = terry };
            Pet whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            Pet bluemoon = new Pet { Name = "Blue Moon", Owner = terry };
            Pet daisy = new Pet { Name = "Daisy", Owner = magnus };

            // Create two lists.
            List<Person> people = new List<Person> { magnus, terry, charlotte, arlene };
            List<Pet> pets = new List<Pet> { barley, boots, whiskers, bluemoon, daisy };
            // Create a list where each element is an anonymous type
            // that contains the person's first name and a collection of
            // pets that are owned by them.
            var query = from person in people
                        join pet in pets on person equals pet.Owner into gj
                        select new PetsOwners { Owner = person.FirstName, Pets = gj.ToList() };


            return View(query);
        }

        [HttpGet]
        public IActionResult PersonsPets()
        {
            Person magnus = new Person { FirstName = "Magnus", LastName = "Hedlund" };
            Person terry = new Person { FirstName = "Terry", LastName = "Adams" };
            Person charlotte = new Person { FirstName = "Charlotte", LastName = "Weiss" };
            Person arlene = new Person { FirstName = "Arlene", LastName = "Huff" };

            Pet barley = new Pet { Name = "Barley", Owner = terry };
            Pet boots = new Pet { Name = "Boots", Owner = terry };
            Pet whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            Pet bluemoon = new Pet { Name = "Blue Moon", Owner = terry };
            Pet daisy = new Pet { Name = "Daisy", Owner = magnus };

            // Create two lists.
            List<Person> people = new List<Person> { magnus, terry, charlotte, arlene };
            List<Pet> pets = new List<Pet> { barley, boots, whiskers, bluemoon, daisy };

            var query = from person in people
                        join pet in pets on person equals pet.Owner into gj
                        from subpet in gj.DefaultIfEmpty()
                        select new PersonsPets { FirstName = person.FirstName, PetName = subpet?.Name ?? String.Empty };

            return View(query);
        }

        [HttpGet]
        public IActionResult GroupJoin2()
        {
            Person magnus = new Person { FirstName = "Magnus", LastName = "Hedlund" };
            Person terry = new Person { FirstName = "Terry", LastName = "Adams" };
            Person charlotte = new Person { FirstName = "Charlotte", LastName = "Weiss" };
            Person arlene = new Person { FirstName = "Arlene", LastName = "Huff" };

            Pet barley = new Pet { Name = "Barley", Owner = terry };
            Pet boots = new Pet { Name = "Boots", Owner = terry };
            Pet whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            Pet bluemoon = new Pet { Name = "Blue Moon", Owner = terry };
            Pet daisy = new Pet { Name = "Daisy", Owner = magnus };

            // Create two lists.
            List<Person> people = new List<Person> { magnus, terry, charlotte, arlene };
            List<Pet> pets = new List<Pet> { barley, boots, whiskers, bluemoon, daisy };

            // Create a list where each element is an anonymous
            // type that contains a person's name and
            // a collection of names of the pets they own.
            var query =
                people.GroupJoin(pets,
                                 person => person,
                                 pet => pet.Owner,
                                 (person, petCollection) =>
                                     new PetsOwners
                                     {
                                         Owner = person.FirstName,
                                         Pets = petCollection.ToList()
                                     });

            return View(query);
        }
    }
}
