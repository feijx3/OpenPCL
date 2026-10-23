using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MeloongCore;
using MeloongCore.Extensions;

namespace PCLCS
{
    [NullableContext(1)]
    [Nullable(0)]
    public class WikiEntry
    {
        public override string ToString()
        {
            return string.Format("{0}&{1}|{2}|{3}，浏览量 {4}", new object[]
            {
                this.Slugs.GetOrDefault(ResourcePlatforms.CurseForge, null),
                this.Slugs.GetOrDefault(ResourcePlatforms.Modrinth, null),
                this.Id,
                this.ChineseName,
                this.Popularity
            });
        }

        public int Id;

        [Nullable(2)]
        public string ChineseName;

        public Dictionary<ResourcePlatforms, string> Slugs = new Dictionary<ResourcePlatforms, string>();

        public int Popularity;

        public static readonly Lazy<List<WikiEntry>> All = new Lazy<List<WikiEntry>>(delegate
        {
            List<string> list = FileUtils.ReadAsLines("Resource\\WikiEntries.txt", false, null, typeof(WikiEntry)).ToList<string>();
            Queue<int> queue = new Queue<int>();
            int num = 0;
            for (;;)
            {
                int num2 = num;
                List<string> list2 = list;
                if (num2 >= list2[list2.Count - 1].Length)
                {
                    break;
                }
                Queue<int> queue2 = queue;
                List<string> list3 = list;
                queue2.Enqueue(int.Parse(list3[list3.Count - 1].Substring(num, 3).ConvertRadix(86, 10)));
                num += 3;
            }
            list.RemoveAt(list.Count - 1);
            List<WikiEntry> list4 = new List<WikiEntry>();
            int num3 = 0;
            foreach (string text in list)
            {
                num3++;
                if (!(text == ""))
                {
                    int num4 = queue.Dequeue();
                    foreach (string text2 in text.Split(new char[] { '\u00a8' }))
                    {
                        WikiEntry wikiEntry = new WikiEntry();
                        string[] array2 = text2.Split(new char[] { '|' });
                        string text3 = array2[0];
                        if (text3.StartsWithF("@", false))
                        {
                            wikiEntry.Slugs.Add(ResourcePlatforms.Modrinth, text3.Replace("@", ""));
                        }
                        else if (text3.EndsWithF("@", false))
                        {
                            wikiEntry.Slugs.Add(ResourcePlatforms.CurseForge, text3.TrimEnd(new char[] { '@' }));
                            wikiEntry.Slugs.Add(ResourcePlatforms.Modrinth, wikiEntry.Slugs[ResourcePlatforms.CurseForge]);
                        }
                        else if (text3.Contains("@"))
                        {
                            string[] array3 = text3.Split(new char[] { '@' });
                            wikiEntry.Slugs.Add(ResourcePlatforms.CurseForge, array3[0]);
                            wikiEntry.Slugs.Add(ResourcePlatforms.Modrinth, array3[1]);
                        }
                        else
                        {
                            wikiEntry.Slugs.Add(ResourcePlatforms.CurseForge, text3);
                        }
                        wikiEntry.Id = num3;
                        wikiEntry.Popularity = num4;
                        if (array2.Length >= 2)
                        {
                            WikiEntry wikiEntry2 = wikiEntry;
                            string[] array4 = array2;
                            wikiEntry2.ChineseName = array4[array4.Length - 1];
                            if (wikiEntry.ChineseName.Contains("*"))
                            {
                                string text4 = wikiEntry.Slugs.Values.First<string>().Replace("-", " ").Capitalize();
                                wikiEntry.ChineseName = wikiEntry.ChineseName.Replace("*", " (" + text4 + ")");
                            }
                        }
                        list4.Add(wikiEntry);
                    }
                }
            }
            return list4;
        });
    }
}
