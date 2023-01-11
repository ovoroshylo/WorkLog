// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Since 2.2 you can also author concise templates with method chaining instead of GraphObject.make
// For details, see https://gojs.net/latest/intro/buildingObjects.html

/*
    get frequencies of words that appear in text
*/
function getWordFrequencies(text) {
    // list of english words to exclude from the wordcloud
    var stopWords = [
        "i",
        "me",
        "my",
        "myself",
        "we",
        "us",
        "our",
        "ours",
        "ourselves",
        "you",
        "your",
        "yours",
        "yourself",
        "yourselves",
        "he",
        "him",
        "his",
        "himself",
        "she",
        "her",
        "hers",
        "herself",
        "it",
        "its",
        "itself",
        "they",
        "them",
        "their",
        "theirs",
        "themselves",
        "what",
        "which",
        "who",
        "whom",
        "whose",
        "this",
        "that",
        "these",
        "those",
        "am",
        "is",
        "are",
        "was",
        "were",
        "be",
        "been",
        "being",
        "have",
        "has",
        "had",
        "having",
        "do",
        "does",
        "did",
        "doing",
        "will",
        "would",
        "should",
        "can",
        "could",
        "ought",
        "i'm",
        "you're",
        "he's",
        "she's",
        "it's",
        "we're",
        "they're",
        "i've",
        "you've",
        "we've",
        "they've",
        "i'd",
        "you'd",
        "he'd",
        "she'd",
        "we'd",
        "they'd",
        "i'll",
        "you'll",
        "he'll",
        "she'll",
        "we'll",
        "they'll",
        "isn't",
        "aren't",
        "aren't",
        "wasn't",
        "weren't",
        "hasn't",
        "haven't",
        "hadn't",
        "doesn't",
        "don't",
        "didn't",
        "won't",
        "wouldn't",
        "shan't",
        "shouldn't",
        "can't",
        "cannot",
        "couldn't",
        "mustn't",
        "let's",
        "that's",
        "who's",
        "what's",
        "here's",
        "there's",
        "when's",
        "where's",
        "why's",
        "how's",
        "a",
        "an",
        "the",
        "and",
        "but",
        "if",
        "or",
        "because",
        "as",
        "until",
        "while",
        "of",
        "at",
        "by",
        "for",
        "with",
        "about",
        "against",
        "between",
        "into",
        "through",
        "during",
        "before",
        "after",
        "above",
        "below",
        "to",
        "from",
        "up",
        "upon",
        "down",
        "in",
        "out",
        "on",
        "off",
        "over",
        "under",
        "again",
        "further",
        "then",
        "once",
        "here",
        "there",
        "when",
        "where",
        "why",
        "how",
        "all",
        "any",
        "both",
        "each",
        "few",
        "more",
        "most",
        "other",
        "some",
        "such",
        "no",
        "nor",
        "not",
        "only",
        "own",
        "same",
        "so",
        "than",
        "too",
        "very",
        "say",
        "says",
        "said",
        "shall",
    ];

    var word =
        /[@a-z0-9]+([-.:/'’\u2032\u00A0\u200C\u200D~]+[@a-z0-9]+)*/gi;

    var frequencies = new Map();
    var result;
    while ((result = word.exec(text)) !== null) {
        var match = result[0].toLowerCase();
        if (stopWords.indexOf(match) > -1) {
            // skip stop words
            continue;
        }
        if (!isNaN(Number(match))) {
            // skip numbers
            continue;
        }

        if (frequencies.has(match)) {
            frequencies.set(match, frequencies.get(match) + 1);
        } else {
            frequencies.set(match, 1);
        }
    }

    var iterator = frequencies[Symbol.iterator]();
    var freqArr = [];
    for (const item of iterator) {
        freqArr.push({ word: item[0], freq: item[1] });
    }

    // sort the frequency array in descending order
    freqArr.sort(function (a, b) {
        return b.freq - a.freq;
    });

    return freqArr;
}

//building word cloud of given text and canvas id
function buildWordCloud(objId = "word_cloud", text) {
    var freqArr = getWordFrequencies(text);

    // convert map to array


    // create an array of nodes, scaled appropriately by frequency
    var words = [];
    var singleOccurrenceCount = 0;
    for (var i = 0; i < freqArr.length; i++) {
        if (freqArr[i].freq === 1) {
            singleOccurrenceCount++;
        }
        // stop creating nodes if we've already added too many that only occurred once, or if we've added 150 already
        if ((i > 25 && singleOccurrenceCount > 0.25 * i) || i > 150) {
            break;
        }
        // scale node size logarithmically with frequency
        var scale = 2 * Math.log(freqArr[i].freq) + 1;
        words.push([freqArr[i].word, scale]);
    }

    var word_cloud = $(`#${objId}`)[0];
    WordCloud(word_cloud, {
        list: words,
        weightFactor: 20,
        ellipticity: 1,
        minRotation: 0,
        maxRotation: 0,
        fontFamily: "Impact, serif",
        // clearCanvas: true,
        // color: '#000',
    });

    /*$(`#${objId}`).on("wordclouddrawn", function (e) {
        var span = $(this).children().last();
        var position = span.css(["left", "top"]);
        span.css({
            left: (250 - span.width() / 2) + 'px',
            top: (250 - span.height() / 2) + 'px',
        });
        span.animate(position, 200);
    });*/

}

//building bar chart of give text and canvas id
function drawBarChart(objId = "word_bar_chart", text) {

    if (text.length === 0) {
        $(`#${objId}`).hide();
        return;
    }
    $(`#${objId}`).show();
    var frequencies = getWordFrequencies(text);

    var ctx = $(`#${objId}`)[0].getContext("2d");

    var config = {
        type: 'horizontalBar',
        data: {
            labels: frequencies.map(freq => freq.word),
            datasets: [
                {
                    label: "word counts",
                    backgroundColor: "rgba(60,141,188,0.8)",
                    borderWidth: 0,
                    hoverBackgroundColor: "rgba(60,141,188,1)",
                    hoverBorderColor: "rgba(60,141,188,1)",
                    data: frequencies.map(freq => freq.freq),
                }
            ]
        },
        options: {
            responsiveAnimationDuration: 1000,
            barStrokeWidth: 1,
            responsive: true,
            maintainAspectRatio: false,
            barShowStroke: false,
            tooltips: {
                titleFontSize: 12,
            },
            legend: {
                display: false
            },
            scales: {
                xAxes: [{
                    display: true,
                    gridLines: {
                        offsetGridLines: true
                    },
                    scaleLabel: {
                        show: false,
                        labelString: ''
                    },
                    ticks: {
                        beginAtZero: true,
                        suggestedMin: 0,
                        suggestedMax: 20
                    }
                }],
                yAxes: [{
                    display: true,
                    gridLines: {
                        offsetGridLines: true
                    },
                    scaleLabel: {
                        show: false,
                        labelString: ''
                    },
                }]
            }
        }
    };
    var myChart = new Chart(ctx, config);
}
