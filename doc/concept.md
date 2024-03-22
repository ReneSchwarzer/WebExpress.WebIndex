![WebExpress](https://raw.githubusercontent.com/ReneSchwarzer/WebExpress.Doc/main/assets/banner.png)

# WebExpress.WebIndex
The index model provides a reverse index to enable fast and efficient searching. A reverse 
index can significantly speed up access to the data. However, creating and storing a 
reverse index requires additional storage space and processing time. The storage requirement 
increases, especially with large amounts of data can be important. Therefore, it is important 
to weigh the pros and cons to achieve the best possible performance. The full-text search in `WebExpress` 
supports the following search options:

- Word search
- Wildcard search
- Phrase search (exact word sequence)
- Proximity search
- Fuzzy search

The indexing process begins with the analysis of documents, where the documents are broken down into smaller units, usually words or phrases. These 
broken-down units are then converted into normalized tokens. Normalization can take various forms, such as converting all letters to lowercase, 
removing punctuation, or reducing words to their stem. In addition, stop words are removed. Stop words are frequently occurring words like "and", 
"is", "in", which typically do not provide added value for the search. These words are filtered out to improve efficiency and reduce storage 
requirements. In many languages, words can appear in different forms that all refer to the same concept. Therefore, techniques such as stemming or 
lemmatization are often applied to reduce different forms of a word to a common representation. These additional steps help improve the accuracy and 
relevance of search results and help keep the index compact and manageable. The normalized tokens are stored in a reverse index. This index is 
structured so that it lists the documents in which each token appears for each token. In addition to the tokens, more information is stored, such 
as the frequency of the tokens or the position of the tokens in the document.
During the search process, the search words are tokenized and normalized in the same way, and then each token is looked up in the reverse index. The 
documents or positions found in the lists of all tokens are the search results. This method allows for a fast and efficient search, as the time-consuming 
part is carried out in advance during the indexing process and the actual search consists only of quick lookup operations in the reverse index.

```
 ┌──────────┐       indexing
 │ document │──────────────┐
 └──────────┘              │
                           ▼
 ┌───────┐ searching ┌───────────┐       ┌────────────┐       ┌──────────────────┐       ╔══════════╗
 │ query │──────────>│ tokenizer │──────>│ normalizer │──────>│ stoppword filter │──────>║ WebIndex ║
 └───────┘           └───────────┘       └────────────┘       └──────────────────┘       ╚══════════╝
     ▲                                                                                         │
     └─────────────────────────────────────────────────────────────────────────────────────────┘
```

In this particular instance, indexing is performed on two documents by executing a series of operations: tokenization, normalization, and stop-word removal. The outcome 
of these operations is a multi-dimensional table, which serves as a representation of the reverse index.

```

 ┌document a────────────────────────────────────────┐      ┌document b────────────────────────────────────────┐
 │ No, fine, no , good, fine, good. You know Marty, │      │ Thanks a lot, kid. Now, of course not, Biff, now,│
 │ you look so familiar, do I know your mother? Hey │      │ I wouldn't want that to happen. I'm gonna ram    │
 │ man, the dance is over. Unless you know someone  │      │ him. Well, Marty, I want to thank you for all    │
 │ else who could play the guitar. Who's are these? │      │ your good advice, I'll never forget it. Doc,     │
 │ Maybe you were adopted.                          │      │ look, all we need is a little plutonium.         │
 └──────────────────────────────────────────────────┘      └──────────────────────────────────────────────────┘
                        │                                                         │
                        │                                                         │
                        ▼                                                         ▼
 ┌normalized document a─────────────────────────────┐      ┌normalized document b─────────────────────────────┐
 │ no fine no good fine good you know marty         │      │ thank a lot kid now of course not biff now       │
 │ you look so familiar do i know your mother hey   │      │ i would not want that to happen i am gonna ram   │
 │ man the dance is over unless you know someone    │      │ him well marty i want to thank you for all       │
 │ else who could play the guitar who is are these  │      │ your good advice i will never forget it doc      │
 │ maybe you were adopted                           │      │ look all we need is a little plutonium           │
 └──────────────────────────────────────────────────┘      └──────────────────────────────────────────────────┘
                        │                                                         │
                        │                                                         │
                        ▼                                                         ▼
 ┌stopword cleaned document a───────────────────────┐      ┌stopword cleaned document b───────────────────────┐
 │ fine good fine good know marty                   │      │ thank lot kid course biff                        │
 │ look familiar know mother                        │      │ would want happen gonna ram                      │
 │ dance unless know someone                        │      │ well marty want thank                            │
 │ else could play guitar these                     │      │ good advice never forget doc                     │
 │ maybe adopted                                    │      │ look need little plutonium                       │
 └──────────────────────────────────────────────────┘      └──────────────────────────────────────────────────┘
                       │                                                         │
                       │                                                         │
                       └───────────────┐                       ┌─────────────────┘
                                       ▼                       ▼
                           ╔WebIndex═══════════════════════════════════════╗
                           ║ Term      │ Fequency │ Documnet │ Position    ║
                           ║═══════════════════════════════════════════════║
                           ║ adopted   │ 1        │ a        │ 211         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ advice    │ 1        │ b        │ 153         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ biff      │ 1        │ b        │ 40          ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ could     │ 1        │ a        │ 156         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ course    │ 1        │ b        │ 28          ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ dance     │ 1        │ a        │ 108         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ doc       │ 1        │ b        │ 183         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ else      │ 1        │ a        │ 147         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ familiar  │ 1        │ a        │ 62          ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ fine      │ 2        │ a        │ 5           ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ forget    │ 1        │ b        │ 22          ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ gonna     │ 1        │ b        │ 87          ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ good      │ 3        │ a        │ 16, 28      ║
                           ║           │          │ b        │ 148         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ guitar    │ 1        │ a        │ 171         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ happen    │ 1        │ b        │ 75          ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ kid       │ 1        │ b        │ 15          ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ know      │ 3        │ a        │ 38, 77, 134 ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ little    │ 1        │ b        │ 211         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ look      │ 2        │ a        │ 54          ║
                           ║           │          │ b        │ 188         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ lot       │ 1        │ b        │ 10          ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ marty     │ 2        │ a        │ 43          ║
                           ║           │          │ b        │ 108         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ maybe     │ 1        │ a        │ 196         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ mother    │ 1        │ a        │ 87          ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ need      │ 1        │ b        │ 201         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ never     │ 1        │ b        │ 166         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ play      │ 1        │ a        │ 162         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ plutonium │ 1        │ b        │ 218         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ ram       │ 1        │ b        │ 93          ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ someone   │ 1        │ a        │ 139         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ thank     │ 2        │ b        │ 0, 125      ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ these     │ 1        │ a        │ 189         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ unless    │ 1        │ a        │ 123         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ want      │ 2        │ b        │ 62, 117     ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ well      │ 1        │ b        │ 102         ║
                           ║───────────┼──────────┼──────────┼─────────────║
                           ║ would     │ 1        │ b        │ 53          ║
                           ╚═══════════════════════════════════════════════╝
                                                  ▲
                                                  │
                                                  │
                                     ┌stopword cleaned query───┐
                                     │ marty play guitar       │
                                     └─────────────────────────┘
                                                  ▲
                                                  │
                                                  │
                                     ┌normalized query─────────┐
                                     │ marty play the guitar   │
                                     └─────────────────────────┘
                                                  ▲
                                                  │
                                                  │
                                     ┌query─────────────────────┐
                                     │ 'Marty play the guitar.' │
                                     └──────────────────────────┘
```

`WebIndex` is an efficient system that combines document store and reverse indices to support a variety of search options. The `IndexDocumentStore` stores all 
instances of a document for quick access, regardless of other persistent storage forms such as databases. On the other hand, the reverse index is created for each 
field `IndexField` of a document unless it is marked with `IndexIgnore`. The field contents are tokenized, normalized, and filtered to create the terms of the reverse 
index. Each term in the reverse index is associated with a posting that contains the IDs of the document instances that contain the term. The position where the term was 
found within the attribute value is stored in the position. There can be multiple positions for each posting. When searching for one or more terms, the IDs of 
the instances and their positions within the attribute values can be determined.

```
 ╔═══════════════════════════════════════ IndexManager ═╗
 ║   ┌──────────┐                                       ║
 ║   │ WebIndex │                                       ║
 ║   └──────────┘                                       ║
 ║        │ 1                                           ║
 ║        │            ╔══════ IndexDocumentStore ═╗    ║
 ║        │ *          ║                           ║    ║
 ║ ┌───────────────┐ 1 ║ * ┌──────┐                ║    ║
 ║ │ IndexDocument │───────│ Item │                ║    ║
 ║ └───────────────┘   ║   └──────┘                ║    ║
 ║        │ 1          ╚═══════════════════════════╝    ║
 ║        │                                             ║
 ║        │ *                                           ║
 ║  ┌────────────┐                                      ║
 ║  │ IndexField │                                      ║
 ║  └────────────┘                                      ║
 ║        │ 1                                           ║
 ║ ╔══════│═════ IndexReverse ═╗                        ║
 ║ ║      │ *                  ║                        ║
 ║ ║  ┌──────┐                 ║                        ║
 ║ ║  │ Term │                 ║                        ║
 ║ ║  └──────┘                 ║                        ║
 ║ ║      │ 1                  ║                        ║
 ║ ║      │                    ║                        ║
 ║ ║      │ *                  ║                        ║
 ║ ║ ┌─────────┐               ║                        ║
 ║ ║ │ Posting │               ║                        ║
 ║ ║ └─────────┘               ║                        ║
 ║ ║      │ 1                  ║                        ║
 ║ ║      │                    ║                        ║
 ║ ║      │ *                  ║                        ║
 ║ ║ ┌──────────┐              ║                        ║
 ║ ║ │ Position │              ║                        ║
 ║ ║ └──────────┘              ║                        ║
 ║ ╚═══════════════════════════╝                        ║
 ╚══════════════════════════════════════════════════════╝
```

# IndexManager
The index manager is a central component of the `WebIndex` system and serves as the primary interface for interacting with the indexing functions. It is responsible 
for managing the various `IndexDocuments` that are created in `WebIndex`. Each `IndexDocument` represents a collection of documents that need to be indexed, and the 
index manager ensures that these documents are indexed correctly and efficiently. In addition, the index manager provides functions for adding, updating, and 
deleting documents in the index. It also enables the execution of search queries on the index and returns the corresponding results. Finally, the index manager 
provides high control over the indexing process by allowing certain fields to be excluded from indexing or determining whether the index should be created in 
main memory or persistently in the file system. An `IndexDocument created in main memory enables faster indexing and searching. However, the number of objects it 
can support is limited and depends on the size of the available main memory. Therefore, it is important to weigh the pros and cons and choose the best solution 
for the specific requirements.

## IndexDocument
An IndexDocument representing a class that implements the `IIndexItem` interface. Each `IndexDocument` contains a collection of fields that hold the data to be 
indexed. These fields can contain various types of data, such as text, numbers, or dates. During the indexing process, the data in these fields are analyzed and 
tokenized, then stored in the reverse index. In addition to the reverse index, an `IndexDocument` also includes a document store for quick access to the existing 
instances. When a search query is made for one or more terms, the IDs of the instances that match the terms are identified in the reverse index and supplemented 
with the corresponding instances in the document store and returned to the searcher.

## IndexField
An `IndexField` is a property (C# property) in an index document that can accommodate various types of values, such as text, numbers, or other data. Each field is 
stored in a reverse index, which converts the field values into terms and associates them with document IDs and positional data in the reverse index. The name and 
type of the field are essential pieces of information used during indexing and searching. If a field is marked with the IndexIgnore attribute, it will be excluded 
from the indexing process.

## IndexStore
In a filesystem where the WebIndex is stored, a process is carried out where an inverted index is created for each field. These indexes are stored as files with 
the `<document name><field name>.wri` extension. In parallel, a special storage area known as the document store `<document name>.wds` is set up for each document. In 
this storage area, the document’s data is redundantly stored to enable quick access.
The structure of these files follows a uniform format that is divided into various segments. Each of these segments is identifiable by a unique address and has a specific 
length. There is the option to specify whether a particular segment should be stored in the cache. If a segment is no longer needed, it can be removed from the main 
memory. These features contribute to efficient use of storage and improve the system performance.

```
         ╔Header═════════╗
  3 Byte ║ "wds" | "wri" ║ identification (magic number) of the file `wds` for IndexDocumentStore and `wri` for IndexReverse
         ╠Allocator══════╣ 
  8 Byte ║ NextFreeAddr  ║ the next free address
  8 Byte ║ FreeListAddr  ║ the address to the list
         ╠Statistic══════╣
  4 Byte ║ Count         ║ the number of elements in the file
         ╠Body═══════════╣
         ║               ║ a variable memory area in which the data is stored
         ╚~~~~~~~~~~~~~~~╝
```

Unused memory areas in the file are represented by the “Free” segment, which is located in the Body-Area variable and forms a linked list. The `Allocator` points 
to the first element of this list.

```
         ╔Free═══════════╗
  4 Byte ║ Length        ║ size of the item in bytes
  8 Byte ║ SuccessorAddr ║ pointer to the address of the next element of a sorted list or 0 if there is no element
         ╚═══════════════╝
```

When new documents are indexed, the new segments are accommodated in a free storage area in the file. Initially, this is the end of the file. In fragmented files, where segments 
have already been deleted, the freed storage areas are reused. The `Allocator` in the header always points to the next free storage space with `NextFreeAddr`.

### Example alloc

In this example, segments 2 and 3 are successively added. It is important to note that segment 1 already exists.

```
       initial               add a segment            add a segment

 0 ╔Header═════════╗        ╔Header═════════╗        ╔Header═════════╗
   ║ "wds" | "wri" ║        ║ "wds" | "wri" ║        ║ "wds" | "wri" ║
 3 ╠Allocator══════╣        ╠Allocator══════╣        ╠Allocator══════╣
   ║ 2             ║        ║ 3             ║        ║ 4             ║
   ║ 0             ║        ║ 0             ║        ║ 0             ║
19 ╠Statistic══════╣        ╠Statistic══════╣        ╠Statistic══════╣
   ║ 1             ║        ║ 2             ║        ║ 3             ║
23 ╠Body═══════════╣        ╠Body═══════════╣        ╠Body═══════════╣
   ║   ┌Seg: 1─┐   ║    =>  ║   ┌Seg: 1─┐   ║    =>  ║   ┌Seg: 1─┐   ║
   ║   │       │   ║        ║   │       │   ║        ║   │       │   ║
   ║   └───────┘   ║        ║   └───────┘   ║        ║   └───────┘   ║
   ╚═══════════════╝        ║   ┌Seg: 2─┐   ║        ║   ┌Seg: 2─┐   ║
                            ║   │   +   │   ║        ║   │       │   ║
                            ║   └───────┘   ║        ║   └───────┘   ║
                            ╚═══════════════╝        ║   ┌Seg: 3─┐   ║
                                                     ║   │   +   │   ║
                                                     ║   └───────┘   ║
                                                     ╚═══════════════╝
```

Free memory slots are stored in a linked list, which represents the free segments in the file. These can be reused for storing new data. Unused segments are replaced with 
the `Free`-Segment, and neighboring free segments are merged. This process creates room for larger segments but may lead to the formation of dead memory spaces too small 
for reuse. Reindexing can eliminate these dead spaces, enhancing memory usage efficiency.

### Example Free

In this example, segments 2, 1 and 4 are sequentially released and consolidated.

```
       initial                  remove 2          remove 1 and merge 1&2          remove 4

 0 ╔Header═════════╗        ╔Header═════════╗        ╔Header═════════╗        ╔Header═════════╗
   ║ "wds" | "wri" ║        ║ "wds" | "wri" ║        ║ "wds" | "wri" ║        ║ "wds" | "wri" ║
 3 ╠Allocator══════╣        ╠Allocator══════╣        ╠Allocator══════╣        ╠Allocator══════╣
   ║ 5             ║        ║ 5             ║        ║ 5             ║        ║ 5             ║
   ║ 0             ║        ║ 2             ║─┐      ║ 1             ║─┐      ║ 1             ║─┐
19 ╠Statistic══════╣        ╠Statistic══════╣ │      ╠Statistic══════╣ │      ╠Statistic══════╣ │
   ║ 3             ║        ║ 2             ║ │      ║ 1             ║ │      ║ 1             ║ │
23 ╠Body═══════════╣        ╠Body═══════════╣ │      ╠Body═══════════╣ │      ╠Body═══════════╣ │
   ║   ┌Seg: 1─┐   ║    =>  ║   ┌Seg: 1─┐   ║ │  =>  ║   ┌Free: 1┐   ║◄┘  =>  ║   ┌Free: 1┐   ║◄┘
   ║   │       │   ║        ║   │       │   ║ │      ║   │   X   │   ║        ║   │       │   ║─┐
   ║   └───────┘   ║        ║   └───────┘   ║ │      ║   │       │   ║        ║   │       │   ║ │
   ║   ┌Seg: 2─┐   ║        ║   ┌Free: 2┐   ║◄┘      ║   │       │   ║        ║   │       │   ║ │
   ║   │       │   ║        ║   │   X   │   ║        ║   │       │   ║        ║   │       │   ║ │
   ║   └───────┘   ║        ║   └───────┘   ║        ║   └───────┘   ║        ║   └───────┘   ║ │
   ║   ┌Seg: 3─┐   ║        ║   ┌Seg: 3─┐   ║        ║   ┌Seg: 3─┐   ║        ║   ┌Seg: 3─┐   ║ │
   ║   │       │   ║        ║   │       │   ║        ║   │       │   ║        ║   │       │   ║ │
   ║   └───────┘   ║        ║   └───────┘   ║        ║   └───────┘   ║        ║   └───────┘   ║ │
   ║   ┌Seg: 4─┐   ║        ║   ┌Seg: 4─┐   ║        ║   ┌Seg: 4─┐   ║        ║   ┌Seg: 4─┐   ║◄┘
   ║   │       │   ║        ║   │       │   ║        ║   │       │   ║        ║   │   X   │   ║
   ║   └───────┘   ║        ║   └───────┘   ║        ║   └───────┘   ║        ║   └───────┘   ║
   ╚═══════════════╝        ╚═══════════════╝        ╚═══════════════╝        ╚═══════════════╝
```

The repurposing of unused segments reduces the space requirements of files, particularly for highly fluctuating index files. This practice not only optimizes storage 
but also enhances the overall performance and efficiency of data management systems. It is especially beneficial in environments where data is frequently updated or 
deleted, leading to a high turnover of index files. By reusing these segments, the system can maintain optimal performance while minimizing the need for additional 
storage space.

### Example realloc

In this example, a new segment 1 is added, utilizing the available free memory space. The free memory space is divided in such a way that the 
Free-Segment is split into two segments. This process allows for the efficient use of memory by allocating only the necessary space for the new segment, while 
preserving the remaining free space for future use.

```
       initial          split 1 into 1&2 and add to 1

 0 ╔Header═════════╗        ╔Header═════════╗
   ║ "wds" | "wri" ║        ║ "wds" | "wri" ║
 3 ╠Allocator══════╣        ╠Allocator══════╣
   ║ 5             ║        ║ 5             ║
   ║ 1             ║─┐      ║ 2             ║─┐
19 ╠Statistic══════╣ │      ╠Statistic══════╣ │
   ║ 1             ║ │      ║ 2             ║ │
23 ╠Body═══════════╣ │      ╠Body═══════════╣ │
   ║   ┌Free: 1┐   ║◄┘  =>  ║   ┌Seg: 1─┐   ║ │
   ║   │       │   ║─┐      ║   │   +   │   ║ │
   ║   │       │   ║ │      ║   └───────┘   ║ │
   ║   │       │   ║ │      ║   ┌Free: 2┐   ║◄┘
   ║   │       │   ║ │      ║   │       │   ║─┐
   ║   └───────┘   ║ │      ║   └───────┘   ║ │
   ║   ┌Seg: 3─┐   ║ │      ║   ┌Seg: 3─┐   ║ │
   ║   │   +   │   ║ │      ║   │       │   ║ │
   ║   └───────┘   ║ │      ║   └───────┘   ║ │
   ║   ┌Free: 4┐   ║◄┘      ║   ┌Free: 4┐   ║◄┘
   ║   │       │   ║        ║   │       │   ║
   ║   └───────┘   ║        ║   └───────┘   ║
   ╚═══════════════╝        ╚═══════════════╝
```

### IndexDocumentStore
A `IndexDocumentStore` is a data structure in which each key is associated with a value. This allows efficient retrieval and retrieval of data based on the key. The 
document store plays a crucial role in improving the efficiency of queries by enabling direct access to the document instances that contain the desired terms. Access 
to the document instances is done via a HashMap, where the ID serves as the key. The internal structure of the document store:

```
         ╔Body═══════════╗
         ║ HashMap       ║ a hashmap in which the data is stored
         ╚~~~~~~~~~~~~~~~╝
```

```
         ╔HashMap════════╗
  4 Byte ║ BucketCount   ║ number of buckets (the next prime number of capacity)
         ╠Data═══════════╣
         ║ Bucket 0      ║ pointer to the address of the first element of a sorted list that has the same hash values (collisions) or 0 if there is no element
  n *    ║ Bucket 1      ║
  8 Byte ║~~~~~~~~~~~~~~~║
         ║~~~~~~~~~~~~~~~║
         ║ Bucket n      ║
         ║               ║ a variable memory area in which the data is stored
         ╚~~~~~~~~~~~~~~~╝
```

The document instances are stored in a segment. The size of the segment is variable and is determined by the size of the compressed document instance. The segment are stored 
in the variable memory area.
```
         ╔Item═══════════╗
 16 Byte ║ Id            ║ guid of the document item
  4 Byte ║ Length        ║ size of the item in bytes
  8 Byte ║ SuccessorAddr ║ pointer to the address of the next element of a sorted list or 0 if there is no element
  n Byte ║ Data          ║ a variable memory area in which the item is stored (gzip compressed)
         ╚═══════════════╝
```

### IndexReverse
A reverse index is a specialized type of index that allows access to data in reverse order. In the context of the `WebIndex`, the reverse index is used for efficient 
searching of terms. These terms are derived from the associated fields, and their values are broken down into tokens, normalized, filtered, and stored in a search 
tree for fast retrieval.

```
         ╔Body═══════════╗
         ║ Term          ║ the root node
         ╚~~~~~~~~~~~~~~~╝
```

```

         ╔Term═══════════╗
 30 Byte ║ TermNode      ║ the root node
         ╠Data═══════════╣
         ║               ║ a variable memory area in which the data is stored
         ╚~~~~~~~~~~~~~~~╝
```

The tree structure enables efficient search and retrieval of terms. Each node in the tree represents a character of the term, and the sequence of characters along the 
path from the root node to a specific node forms the corresponding term. The `TermNode` segments in the data area of the reverse index are organized in such a way that 
they enable a fast and accurate search. A term segment contains important metadata. This includes the frequency of the term’s occurrence and a reference to a linked list. This list contains the 
documents in which the term appears.

```
         ╔TermNode═══════╗
  2 Byte ║ Character     ║ a character from the term
  8 Byte ║ SiblingAddr   ║ address of the first sibling node or 0 if no sibling node exists
  8 Byte ║ ChildAddr     ║ address of the first child node or 0 if not present
  4 Byte ║ Fequency      ║ the number of times the term is used 
  8 Byte ║ PostingAddr   ║ adress of the first posting element of a sorted list or 0 if there is no element
         ╚═══════════════╝
```

The posting segment is designed as a list and contains the IDs of the documents that belong to a term. For each document, the posting segment refers to the position 
information that indicates where the term is located in the document. The posting segment is stored in the variable memory area of the inverted index.

```
         ╔Posting════════╗
 16 Byte ║ Id            ║ guid of the document item
  8 Byte ║ SuccessorAddr ║ pointer to the address of the next element of a sorted list or 0 if there is no element
  8 Byte ║ PositionAddr  ║ adress of the first position element of a sorted list or 0 if there is no element
         ╚═══════════════╝
```

The position segments form a linked list containing the position information of the associated terms. The position of a term refers to its original 
occurrence in the field value of a document. Each position segment has a fixed size and is created in the variable data area of the reverse index. This 
structure allows for efficient searching and retrieval of terms based on their position in the documents.

```
         ╔Position═══════╗
  4 Byte ║ Position      ║ the position
  8 Byte ║ SuccessorAddr ║ pointer to the address of the next element of a sorted list or 0 if there is no element
         ╚═══════════════╝
```

### Caching
Caching is an efficient technique for optimizing data access by enabling fast access to frequently used data and simultaneously reducing the load on 
the file system. It stores frequently used data in memory, which speeds up access to this data as it does not have to be retrieved from the hard drive 
again. For write accesses, the data is first written to the read cache. They are then queued before being written to a disk by a thread. This process, 
also known as write delay, can improve system performance by decoupling write operations and writing them to the disk at a more favorable time. The 
read cache uses a hash map to allow random access to the cached segments. Each cached segment has a defined lifetime. If this has expired, the segments 
are removed from the read cache, unless they have been marked as immortal via the `SegmentCached` attribute.

## Indexing

Indexing is a crucial process that enables quick information retrieval. The index is created from the values of the document fields. This index is stored 
on the file system and is updated whenever a document value is added or changed. Sometimes it is necessary to manually regenerate the index, for example, 
when a new document field is added or when the index is lost or damaged. The reindexing deletes all indexes and recreates them.

```csharp
public class Greetings : IIndexItem
  [IndexIgnore]
  public Guid Id { get; set; }
  
  public string Text { get; set; }
}
 
// somewhere in the code...
IndexManager.Register<Test>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

var greetings = new []
{
    new Greetings { Id = new Guid("b2e8a5c3-1f6d-4e7b-9e1f-8c1a9d0f2b4a"), Text = "Hello Helena!"},
    new Greetings { Id = new Guid("c7d8f9e0-3a2b-4c5d-8e6f-9a1b0c2d4e5f"), Text = "Hello Helena, Helge & Helena!"}
};

IndexManager.ReIndex(greetings);
```

```
 ┌Term: 23┐
 │ null   │ root
 │ 0      │ 
 │ 53     │────►┌Term: 53┐
 │ 0      │     │ 'h'    │ first letter from helge and helena
 │ 0      │     │ 0      │
 └────────┘     │ 83     │────►┌Term: 83┐
                │ 0      │     │ 'e'    │ second letter from helge and helena
                │ 0      │     │ 0      │
                └────────┘     │ 113    │────►┌Term:113┐
                               │ 0      │     │ 'l'    │ third letter from helge and helena
                               │ 0      │     │ 0      │
                               └────────┘     │ 321    │────►┌Term:143┐
                                              │ 0      │     │ 'e'    │ fourth letter from helena
                                              │ 0      │  ┌──│ 321    │
                                              └────────┘  │  │ 173    │────►┌Term:173┐
                                                          │  │ 0      │     │ 'n'    │ fifth letter from helena
                                                          │  │ 0      │     │ 0      │
                                        ┌Term:321┐◄───────┘  └────────┘     │ 203    │────►┌Term:203┐
                                        │ 'g'    │ fourth letter from helge │ 0      │     │ 'a'    │ sixth letter from helena
                                        │ 0      │                          │ 0      │     │ 0      │
                                        │ 351    │────►┌Term:351┐           └────────┘     │ 0      │
                                        │ 0      │     │ 'e'    │ fifth letter from helge  │ 2      │
                                        │ 0      │     │ 0      │                        ┌─│ 233    │
                                        └────────┘     │ 0      │                        │ └────────┘
                                                       │ 1      │                        │
                                                     ┌─│ 381    │                        │
                                                     │ └────────┘                        ▼
                                                     ▼                                  ┌Post:233┐
                                                    ┌Post:381┐                          │ 'b2..' │
                                                    │ 'c7..' │                          │ 277    │────►┌Post:277┐
                                                    │ 0      │           ┌Pos: 265┐◄────│ 265    │     │ 'c7..' │
                                     ┌Pos: 413┐◄────│ 413    │           │ 1      │     └────────┘     │ 0      │
                                     │ 3      │     └────────┘           │ 0      │                    │ 309    │────►┌Pos: 309┐
                                     │ 0      │                          └────────┘                    └────────┘     │ 1      │
                                     └────────┘                                                        ┌Pos: 425┐◄────│ 425    │
                                                                                                       │ 4      │     └────────┘
                                                                                                       │ 0      │
                                                                                                       └────────┘
```

## Searching


# WQL
The WebExpress Query Language (WQL) is a query language that filters and sorts a given amount of data from the reverse index. A statement of the query language 
is usually sent from the client to the server, which collects, filters and sorts the data in the reverse index and sends it back to the client.
The following BNF is used to illustrate the grammar:

```
<WQL>                  ::= <Filter> <Order> <Partitioning> | ε
<Filter>               ::= "(" <Filter> ")" | <Filter> <LogicalOperator> <Filter> |<Condition> | ε
<Condition>            ::= <Attribute> <BinaryOperator> <Parameter> | <Attribute> <SetOperator> "(" <Parameter> <ParameterNext> ")"
<LogicalOperator>      ::= "and" | "or"
<Attribute>            ::= <Name>
<Function>             ::= <Name> "(" <Parameter> <ParameterNext> ")" | Name "(" ")"
<Parameter>            ::= <Function> | <DoubleValue> | """ <StringValue> """ | "'" <StringValue> "'"  | <StringValue>
<ParameterNext>        ::= "," <Parameter> <ParameterNext> | ε
<BinaryOperator>       ::= "=" | ">" | "<" | ">=" | "<=" | "!=" | "~" | "is" | "is not"
<SetOperator>          ::= "in" | "not in"
<Order>                ::= "order" "by" <Attribute> <DescendingOrder> <OrderNext> | ε
<OrderNext>            ::= "," <Attribute> <DescendingOrder> <OrderNext> | ε
<DescendingOrder>      ::= "asc" | "desc" | ε
<Partitioning>         ::= <Partitioning> <Partitioning> | <PartitioningOperator> <Number> | ε
<PartitioningOperator> ::= "take" | "skip"
<Name>                 ::= [A-Za-z_.][A-Za-z0-9_.]+
<StringValue>          ::= [A-Za-z0-9_@<>=~$%/!+.,;:\-]+
<DoubleValue>          ::= [+-]?[0-9]*[.]?[0-9]+
<Number>               ::= [0-9]+
```

## Term modifiers
Term modifiers in WQL are special characters or combinations of characters that serve to modify search terms, thus offering a wide range of search 
possibilities. The use of term modifiers can contribute to improving the efficiency and accuracy of the search. They can be used to find exact matches 
for phrases, to search for terms that match a certain pattern, to search for terms that are similar to a certain value, and to search for terms that are 
near another term. Term modifiers are an essential part of WQL and contribute to increasing the power and flexibility of the search. They allow users to 
create customized search queries tailored to their specific requirements. It is important to note that all queries are case-insensitive. This means that 
the case is not taken into account in the search, which simplifies the search and improves user-friendliness.

**Phrase search (exact word sequence)**

Phrase search allows users to retrieve content from documents that contain a specific order and combination of words defined by the user. With phrase 
search, only records that contain the expression in exactly the searched order are returned. For this, the position information of the reverse index is used.

`Description = 'lorem ipsum'`

**Proximity search**

A proximity search looks for documents where two or more separately matching terms occur within a certain distance of each other. The distance is determined by 
the number of intervening words. Proximity search goes beyond simple word matching by adding the constraint of proximity. By limiting proximity, search results can 
be avoided where the words are scattered and do not cohere. The basic linguistic assumption of proximity search is that the proximity of words in a document implies 
a relationship between the words.

`Description ~2 'lorem ipsum'

**Wildcard search**

A wildcard search is an advanced search technique used to maximize search results. Wildcards are used in search terms to represent one or more other characters.

- An asterisk `*` can be used to specify any number of characters.
- A question mark `?` can be used to represent a single character anywhere in the word. It is most useful when there are variable spellings for a word and 
  you want to search all variants at once.
- A tilde `~` can be used to find strings that approximately match a given term."


`Description = '?orem'`
`Description = 'ips*'`
`Description = 'ips~'`

**Word search**

Word search is the search for specific terms in a document, regardless of their capitalization or position. This concept is particularly useful when searching for 
specific terms in a document without having to pay attention to their exact spelling or occurrence in the document. It enables efficient searches for specific terms.

`Description ~ 'lorem ipsum'`
