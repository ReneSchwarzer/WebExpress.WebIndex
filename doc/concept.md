![WebExpress](https://raw.githubusercontent.com/ReneSchwarzer/WebExpress.Doc/main/assets/banner.png)

# WebExpress.WebIndex
The index model provides a reverse index to enable fast and efficient searching. A reverse 
index can significantly speed up access to the data. However, creating and storing a 
reverse index requires additional storage space and Processing time. The storage requirement 
increases, especially with large amounts of data can be important. Therefore, it is important 
to weigh the pros and cons to achieve the best possible performance. The full-text search in WebExpress 
supports the following search options:

- Word search
- Wildcard search
- Phrase search (exact word sequence)
- Proximity search
- Fuzzy search

`WebIndex` is an efficient system that combines docuemte store and reverse indices to support a variety of search options. The `IndexDocumentStore` stores all 
instances of a document for quick access, regardless of other persistent storage forms such as databases. On the other hand, the reverse index is created for each 
field (`IndexField`) of a document unless it is marked with `IndexIgnore`. The field contents are tokenized, normalized, and filtered to create the terms of the reverse 
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

## IndexDocumentStore
A `IndexDocumentStore` is a data structure in which each key is associated with a value. This allows efficient retrieval and retrieval of data based on the key. The 
document store plays a crucial role in improving the efficiency of queries by enabling direct access to the document instances that contain the desired terms. Access 
to the document instances is done via a HashMap, where the ID serves as the key. The internal structure of the document store:

```
        ╔Header════════╗
 3 Byte ║ "wds"        ║ identification of the file
        ╠Allocator═════╣
 8 Byte ║ NextFreeAddr ║ the next available address
        ╠Statistic═════╣
        ║              ║
        ╠HashMap═══════╣
 4 Byte ║ BucketCount  ║ number of buckets (the next prime number of capacity)
        ║ Bucket 0     ║ pointer to the address of the first element of a sorted list that has the same hash values ​​(collisions). 0 if there is no element.
 n *    ║ Bucket 1     ║
 8 Byte ║~~~~~~~~~~~~~~║
        ║~~~~~~~~~~~~~~║
        ║ Bucket n     ║
        ╠Data══════════╣
        ║              ║
        ╚vvvvvvvvvvvvvv╝
```

## IndexField
An `IndexField` is a property (C# property) in an index document that can accommodate various types of values, such as text, numbers, or other data. Each field is stored in 
a reverse index, which converts the field values into terms and associates them with document IDs and positional data in the reverse index. The name and type of the 
field are essential pieces of information used during indexing and searching. If a field is marked with the IndexIgnore attribute, it will be excluded from the indexing process.

## IndexReverse
A reverse index is a specialized type of index that allows access to data in reverse order. In the context of the WebIndex, the reverse index is used for efficient 
searching of terms. These terms are derived from the associated fields, and their values are broken down into tokens, normalized, filtered, and stored in a search 
tree for fast retrieval.

```
        ╔Header════════╗
 3 Byte ║ "wri"        ║ identification of the file
        ╠Allocator═════╣
 8 Byte ║ NextFreeAddr ║ the next available address
        ╠Statistic═════╣
        ║              ║
        ╠TreeNode══════╣
        ║ RootNode     ║
        ╠Data══════════╣
        ║              ║
        ╚vvvvvvvvvvvvvv╝
```
### Term

```
        ╔TreeNode═══════╗
 8 Byte ║ SuccessorAddr ║ the sibling node or 0 if no sibling node exists
 1 Byte ║ Character     ║ a character from the term
 8 Byte ║ Term          ║ address of the term or null if not present
 8 Byte ║ HeadAddr      ║ address of the first child node or null if not present
        ╚═══════════════╝
```

```
        ╔Term══════════╗
 4 Byte ║ Fequency     ║ the number of times the term is used 
        ╠HashMap═══════╣
 4 Byte ║ BucketCount  ║ number of buckets (the next prime number of capacity)
        ║ Bucket 0     ║ pointer to the address of the first element of a sorted list that has the same hash values ​​(collisions). 0 if there is no element.
 n *    ║ Bucket 1     ║
 8 Byte ║~~~~~~~~~~~~~~║
        ║~~~~~~~~~~~~~~║
        ║ Bucket n     ║
        ╚══════════════╝
```

### Posting

### Position
       
### Example
"Hello Helena"
```       
 ┌Addr: 0─┐
 │ 0      │
 │ null   │ root
 │ 0      │        
 │ 1      │────>┌Addr: 1─┐
 └────────┘     │ 0      │
                │ 'h'    │ first letter from Hello and Helena
                │ 0      │    
                │ 2      │────>┌Addr: 2─┐
                └────────┘     │ 0      │
                               │ 'e'    │ second letter from Hello and Helena
                               │ 0      │
                               │ 3      │────>┌Addr: 3─┐
                               └────────┘     │ 0      │                                                      
                                              │ 'l'    │ third letter from Hello and Helena                   
                                              │ 0      │                                                      
                                              │ 4      │────>┌Addr: 4─┐                                         
                                              └────────┘     │ 5      │────────────────────────────────────────>┌Addr: 5─┐     
                                                             │ 'e'    │ fourth letter from Helena               │ 0      │     
                                                             │ 0      │                                         │ 'l'    │ fourth letter from Hello   
                                                             │ 8      │────>┌Addr: 8─┐                          │ 0      │     
                                                             └────────┘     │ 0      │                          │ 6      │────>┌Addr: 6─┐         
                                                                            │ 'n'    │ fifth letter from Helena └────────┘     │ 0      │         
                                                                            │ 0      │                                         │ 'o'    │ fifth letter from Hello
                                                                            │ 9      │────>┌Addr: 9─┐                          │ 7      │─────────┐    
                                                                            └────────┘     │ 0      │                          │ 0      │         │
                                                                                           │ 'a'    │ sixth letter from Helena └────────┘         V
                                                                                           │ 10     │─────────┐                               ┌Addr: 7─┐
                                                                                           │ 0      │         │                               │ Term   │
                                                                                           └────────┘         V                               └────────┘
                                                                                                           ┌Addr: 10┐
                                                                                                           │ Term   │
                                                                                                           └────────┘
```       
