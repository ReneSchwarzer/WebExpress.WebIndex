using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using WebExpress.WebIndex.Term.Pipeline;

namespace WebExpress.WebIndex.Term
{
    /// <summary>
    /// The analyzer decomposes and processes the input string into a sequence of terms.
    /// </summary>
    public sealed class IndexTokenAnalyzer : IDisposable
    {
        /// <summary>
        /// Returns or sets the index context.
        /// </summary>
        public IIndexContext Context { get; private set; }

        /// <summary>
        /// Returns the whitespace tokinizer.
        /// </summary>
        private static IndexTermTokenizer Tokenizer { get; } = new IndexTermTokenizer();

        /// <summary>
        /// Retruns the pipeline. The pipeline represents a sequence of processing stages. Each 'PipeStage' in this pipeline performs a 
        /// specific task, such as stemming, lemmatization, or stopword filtering. The data is sequentially passed through each 'PipeStage',
        /// with each stage applying its specific processing to the data.
        /// </summary>
        private List<IIndexPipeStage> TextProcessingPipeline { get; } = new List<IIndexPipeStage>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public IndexTokenAnalyzer(IIndexContext context)
        {
            Context = context;

            Initialization();
        }

        /// <summary>
        /// Initialization
        /// </summary>
        private void Initialization()
        {
            var assembly = typeof(IndexManager).Assembly;
            string[] fileNames = 
            [
                "IrregularWords.en", "IrregularWords.de", 
                "MisspelledWords.en", "MisspelledWords.de", 
                "RegularWords.en", "RegularWords.de", 
                "StopWords.en", "StopWords.de",
                "Synonyms.en", "Synonyms.de"
            ];

            Directory.CreateDirectory(Context.IndexDirectory);

            foreach (var fileName in fileNames)
            {
                var path = Path.Combine(Context.IndexDirectory, fileName.ToLower());
                var resources = assembly.GetManifestResourceNames();
                var resource = resources
                    .Where(x => x.EndsWith($".{fileName}", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (resource == null)
                {
                    continue;
                }

                try
                {
                    if (!File.Exists(path))
                    {
                        using var sw = new StreamWriter(path, false, Encoding.UTF8);
                        using var contentStream = assembly.GetManifestResourceStream(resource);
                        using var sr = new StreamReader(contentStream, Encoding.UTF8);

                        sw.Write(sr.ReadToEnd());
                    }
                }
                catch
                {
                }
            }

            Register(new IndexPipeStageConverterTrim(Context));
            Register(new IndexPipeStageConverterLowerCase(Context));
            Register(new IndexPipeStageConverterMisspelled(Context));
            Register(new IndexPipeStageConverterNormalizer(Context));
            Register(new IndexPipeStageConverterSingular(Context));
            Register(new IndexPipeStageConverterSynonym(Context));
            Register(new IndexPipeStageFilterEmpty(Context));
            Register(new IndexPipeStageFilterStopWord(Context));
        }

        /// <summary>
        /// Registers a pipe state for processing the tokens.
        /// </summary>
        /// <param name="pipeState">The pipe stage to add.</param>
        public void Register(IIndexPipeStage pipeStage)
        {
            TextProcessingPipeline.Add(pipeStage);
        }

        /// <summary>
        /// Analyze the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="retrieval">Is set if the analysis is used for queries and the placeholders should not be treated as separators.</param>
        /// <returns>The terms.</returns>
        public IEnumerable<IndexTermToken> Analyze(string input, CultureInfo culture, bool retrieval = false)
        {
            var token = Tokenizer.Tokenize(input, retrieval ? IndexTermTokenizer.Wildcards : null);

            foreach(var pipeStage in TextProcessingPipeline)   
            {
                token = pipeStage.Process(token, culture);
            } 

            return token;
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
