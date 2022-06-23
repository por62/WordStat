using System.Text;

namespace WordStat6.xUnit.Test
{
	public class WordStat_Tests
	{
		public static string _Text =
@"His mum had told him the story of Br'er Rabbit and the Tar-Baby when he was little. He had been terrified, picturing the glistening man made of tar that Br'er Rabbit had fought, the animal becoming more and more glued to his opponent with every blow. It had given him nightmares for weeks, dreams of a hot, black embrace and steaming mouths lowering down onto his…
He shouted for help, tipping his head back and bellowing even as the tarmac gave up all pretence of solidity and sucked him straight down. The shout was cut off in his throat as the ground suddenly hardened again, his teeth slamming onto solid pavement in splinters of enamel. He couldn't cry out at the pain, the earth and rubble in his throat choked all hope of that. Nothing could get past it, most certainly not air.
It took him longer than he might have liked to die.
Все разошлись. Убрав всё, Олег сел на старенький стул и включил компьютер. Зашуршал винчестер, экран озарился различными быстро сменяющимися надписями. Делать ничего не хотелось, почта была прочитана, лимит исчерпан и до конца месяца можно было сосредоточиться на чём-то другом. С десктопа, который он украшал исключительно фотографиями, приветливо улыбалась одна из его любимых актрис. 
Его взгляд непроизвольно задержался на глазах актрисы. Как раз на этой фотографии глаза были очень хорошо видны и было заметно, что они очень напоминают глаза, которые он увидел в видении. ""Интересно, что же это всётаки было. Hадо всё же будет сходить посмотреть, что это за Посланник."" Глаза на экране блестели, наливались жизнью, а потом стали изменяться... 
Глаза Олега метнулись и стало понятно, что изменялась вся картинка - одежда стала медленно выцветать, кожа приобретала мертвенно-бледный оттенок. Из глаз же, прямо из глубины зрачков, стала сочиться тёмно-красная кровь. Вначале эти капли были совсем крохотные, но они всё скапливалась, а потом, преодолев, видно, критическую черту, потекли вниз тягучей массой, застыли ненадолго, а потом полетели вниз, с неохотой отрываясь с нижних век. 
Они летели медленно, как будто что-то удерживало их и покрывали теперь уже абсолютно серую одежду краными пятнами, которые тут же выцветали и становились пепельно-чёрными. Кровь же не останавливаясь текал из чёрной глубины. Hарушая все законы, она стала расползаться по лицу образуя некое подобие красной маски. 
Голова, находящаяся до того в полунаклонённом положении выпрямилась и глаза, красные глаза, ярко блестящие в полумраке, стали завораживать Олега, который тщетно пытался хоть что-то сделать. 
Губы, ставшие ярко-алыми, представляя яркий контраст с бледной кожей, раскрылись и прошептали одно слово ""Опасайся..."", после чего экран перерезала трещина, кожа зашелушилась, почернела и обнажила череп, на котором жутко улыбались всё те же ярко-алые губы, и пронзительно смотрели глаза, удерживавшие в воздухе кровавую маску. Руки, белыми костями вскинултсь вверх в полумольбе-полузаклинании, кровь на одежде вспыхнула, и клубы дыма стали медленно покрывать экран. 
Олег стряхнул оцепенение и увидел всё тот же экран, с которого всё так же приветливо улыбалась его любимая актриса. ""Фильмы надо меньше смотреть"", - подумал Олег и нервно нащупав ""Power"" вдавил кнопку до отказа... ";

		[Fact()]
		public void LangDetector()
		{
			var textRus = "проект";
			var textEn = "project";

			var langDetector = new LangDetector();


			Assert.Equal(Lang.Russian, langDetector.GetLanguage(textRus));
			Assert.Equal(Lang.English, langDetector.GetLanguage(textEn));
		}

		[Fact()]
		public void WordBreakers()
		{
			IWordBreaker wb1 = new WordBreakerRegEx(3);
			IWordBreaker wb2 = new WordBreakerByDelimiters(3);

			var words1 = wb1.GetWords(_Text).OrderBy(w => w).ToArray();
			var words2 = wb2.GetWords(_Text).OrderBy(w => w).ToArray();

			Assert.All(
				Enumerable.Range(0, Math.Min(words1.Length, words2.Length)),
				i => Assert.Equal(words1[i], words2[i]));

			Assert.Equal(words1.Length, words2.Length);
		}

		[Fact()]
		public void WordBreakers2()
		{
			IWordBreaker wb1 = new WordBreakerRegEx(5);
			IWordBreaker wb2 = new WordBreakerByDelimiters(5);

			var text = "{cooler;false}";

			Assert.Contains(wb1.GetWords(text), t => t == "cooler");
			Assert.Contains(wb1.GetWords(text), t => t == "false");

			Assert.Contains(wb2.GetWords(text), t => t == "cooler");
			Assert.Contains(wb2.GetWords(text), t => t == "false");

		}


		[Fact()]
		public void WordCounters()
		{
			IWordBreaker wbr = new WordBreakerRegEx(3);
			IWordBreaker wbd = new WordBreakerByDelimiters(3);

			using (var wc = new WordCounterMT(wbd, new MemoryStream(Encoding.UTF8.GetBytes(_Text)), Encoding.UTF8, 2))
				Assert.Equal(56, Top10WordCountSum(wc));

			using (var wc = new WordCounterST(wbd, new MemoryStream(Encoding.UTF8.GetBytes(_Text)), Encoding.UTF8))
				Assert.Equal(56, Top10WordCountSum(wc));

			using (var wc = new WordCounterMTm(wbd, new MemoryStream(Encoding.UTF8.GetBytes(_Text)), Encoding.UTF8, 2, 10))
				Assert.Equal(56, Top10WordCountSum(wc));

			using (var wc = new WordCounterST(wbr, new MemoryStream(Encoding.UTF8.GetBytes(_Text)), Encoding.UTF8))
				Assert.Equal(56, Top10WordCountSum(wc));

			using (var wc = new WordCounterMT(wbr, new MemoryStream(Encoding.UTF8.GetBytes(_Text)), Encoding.UTF8, 2))
				Assert.Equal(56, Top10WordCountSum(wc));

			using (var wc = new WordCounterMTm(wbr, new MemoryStream(Encoding.UTF8.GetBytes(_Text)), Encoding.UTF8, 2, 10))
				Assert.Equal(56, Top10WordCountSum(wc));

		}

		private static int Top10WordCountSum(IWordCounter wc)
		{
			var result = wc.Count(null);

			var count = result
				.OrderByDescending(a => a.Value)
				.Take(10)
				.Sum(a => a.Value);

			return count;
		}
	}
}
