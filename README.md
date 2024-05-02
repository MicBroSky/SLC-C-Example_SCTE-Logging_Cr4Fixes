# SCTE-35 Hexadecimal Parsing & Offloading To Indexing Database Example

## About

Shows an example on how to use the [Skyline.DataMiner.Utils.SCte](https://github.com/SkylineCommunications/SLC-C-Example_SCTE-Logging) NuGet to both parse a hexadecimal file and offload the file to an indexing database set by your DMA. 

This protocol demonstrates how SCTE Events should be filled into your configured indexing database.
This is achieved through using a "Create SCTE Events" button where you can fill in various parts of the event which would be set inside of a protocol. 
After confirming these values, the SCTE Event is then processed and sent to your configured indexing database. Most of the values can be filled by using a hexadecimal string given by a SCTE command which this protocol uses a NuGet package that translates the hex code. 
Once on your configured indexing database, you can use the ID of the protocol and the table's ID (80000000) in order to find the values from the table. The table is hidden as you are expected to read the table data from the indexing database instead of on the element.  

The table features the following columns: 
> scte_key (Key)
	An autoincremented value that starts from 0 and saves it placement between reboots.

> scte_ts (Timestamp)
	A UTC timestamp based on the SCTE signal.

> scte_opid (Operation ID) 
	The value of the opid (operation ID) field on the SCTE object's structure. This is sometimes not given in a device's SCTE implementation.

> scte_opname (Operation Name)
	A text representation of the operation ID.

> scte_src (Source) 
	It is a combination of the name that the SCTE Hexadecimal might have provided and the IP address.

> scte_str (Stream)
	A representation of the source that the SCTE came from. It is usually provided by the SCTE Hexadecimal or from the device.

> scte_pgm (Program)
	A representation of the device that the SCTE came from. It is usually the DataMiner Element's name.

> scte_obj (Object) 
	A JSON representation of the SCTE 35 or SCTE104 operation used in this row.

> scte_segevntid (Segmentation Event ID)
	A segmentation event identifier.

> scte_segupid (Segmentation UPID) 
	The Unique Program Identifier for the SCTE Event. It is separated by a '_' where the first part shows the service it came from through a string and the second part just being an unique ID. Ex: "telemundo_EPOO1825960702"

> scte_segtypeid (Segmentation Type ID) 
	An ID that corresponds to the type of starting or ending event being used.

> scte_segtypename (Segmenttion Type Name)
	The Name of the SCTE Event.

> scte_fld1 (Field 1)
	An extra field to fill in any additional information. 

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.

<!-- Uncomment below and add more info to provide more information about how to use this package. -->
<!-- ## Getting Started -->
