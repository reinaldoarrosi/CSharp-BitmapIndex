#Bitmap Index (C#)

This is a bitmap indexing library based on the amazing EWAH bitmap compression library available at https://github.com/lemire/csharpewah

There is also a Java version: https://github.com/reinaldoarrosi/bitmapindex

##Installing

- Clone this repository
- Open solution in Visual Studio 2010 or later
- Build solution
- Add a reference to the BitmapIndex.dll and to the EWAH.dll available in the libs folder
- Have fun!

##Usage

###Creating the index
The index is a simple key-value pair where each key is an instance of BIKey and each value is a bitmap (compressed through the EWAH Bitmap lib).

Suppose we have the following table:
<table border="1" cellpadding="5">
    <tr>
        <th>
            Printer
        </th>
        <th>
            Brand
        </th>
        <th>
            # of cartridges
        </th>
        <th>
            Pages/second
        </th>
        <th>
            DPI
        </th>
    </tr>
    <tr>
        <td>
            Model 1
        </td>
        <td>
            HP
        </td>
        <td>
            2
        </td>
        <td>
            20
        </td>
        <td>
            300
        </td>
    </tr>
    <tr>
        <td>
            Model 2
        </td>
        <td>
            HP
        </td>
        <td>
            3
        </td>
        <td>
            30
        </td>
        <td>
            700
        </td>
    </tr>
    <tr>
        <td>
            Model 3
        </td>
        <td>
            Cannon
        </td>
        <td>
            1
        </td>
        <td>
            40
        </td>
        <td>
            N/A
        </td>
    </tr>
    <tr>
        <td>
            Model 4
        </td>
        <td>
            Samsung
        </td>
        <td>
            2
        </td>
        <td>
            40
        </td>
        <td>
            N/A
        </td>
    </tr>
    <tr>
        <td>
            Model 5
        </td>
        <td>
            Cannon
        </td>
        <td>
            2
        </td>
        <td>
            20
        </td>
        <td>
            400
        </td>
    </tr>
</table>

To create the index that represents this dataset we would do:
    
    // Attributes constants
    public static int BRAND = 1;
    public static int CARTRIDGES = 2;
    public static int PPS = 3;
    public static int DPI = 4;
    
    // Brands constants
    public static int HP = 1;
    public static int CANNON = 2;
    public static int SAMSUNG = 3;
    
    BitmapIndex index = new BitmapIndex();
    index.Set(new BIKey(BRAND, HP), 0);
    index.Set(new BIKey(BRAND, HP), 1);
    index.Set(new BIKey(BRAND, CANNON), 2);
    index.Set(new BIKey(BRAND, SAMSUNG), 3);
    index.Set(new BIKey(BRAND, CANNON), 4);
    
    index.Set(new BIKey(CARTRIDGES, 2), 0);
    index.Set(new BIKey(CARTRIDGES, 3), 1);
    index.Set(new BIKey(CARTRIDGES, 1), 2);
    index.Set(new BIKey(CARTRIDGES, 2), 3);
    index.Set(new BIKey(CARTRIDGES, 2), 4);
    
    index.Set(new BIKey(PPS, 20), 0);
    index.Set(new BIKey(PPS, 30), 1);
    index.Set(new BIKey(PPS, 40), 2);
    index.Set(new BIKey(PPS, 40), 3);
    index.Set(new BIKey(PPS, 20), 4);
    
    index.Set(new BIKey(DPI, 300), 0);
    index.Set(new BIKey(DPI, 700), 1);
    index.Set(new BIKey(DPI, 400), 4);

Each BIKey is composed of two parts: group and value. The group represents an attribute in our dataset, and the value represents the many values that an atribute might have. Each group of a BIKey can (optionally) be splitted into 2 fragments (group and subgroup - BIKey.BIGroup class), this allows for more flexibility but almost never necessary.

Each bit in a bitmap represents a row in our table. Each BIKey is an represents the possible combinations of attributes and values, so for each of theses combination a bitmap will be created.

With the index created we can query it though the BICriteria class. BICriteria contains some static methods that are used to create the criteria for our query. These criterias can also be joined with 'ands' and 'ors' to produce more complex criterias.

An example is to find printers where the brand is HP and with a printing performance of 20 or 30 pages per second.
    
    BICriteria criteria = BICriteria.equals(new BIKey(BRAND, HP))
        .and(BICriteria.equals(new BIKey(PPS, 20))
            .or(BICriteria.equals(new BIKey(PPS, 30))))
            
    EWAHCompressedBitmap result = index.query(criteria);
    
The query returns an EWAHCompressedBitmap and we can use the **GetPositions()** method to retrieve the bits that are set, which in turn matches the rows that satisfies our criteria.

Thanks to the amazing compression offered by the EWAH compression library this index scales pretty well and can be kept in-memory even with millions of rows in the dataset. The speed is amazing due to the fact that the criteria evaluation is simply a binary operation.