============================================================
XR Assembly
============================================================


Visualize Assembly data in XR


Main features
-------------

* feature
* feature
* more features

**xr_assembly** runs on Python x.x and x.x.


Documentation
-------------

.. Explain how to access documentation: API, examples, etc.

..
.. optional sections:

Requirements
------------

.. Write requirements instructions here


Dependencies
------------

* `Assembly Information Model <https://github.com/augmentedfabricationlab/assembly_information_model>`_


Installation of Dependencies
----------------------------
**Assembly Information Model**
::
    (your_env_name) python -m pip install git+https://github.com/augmentedfabricationlab/assembly_information_model@master#egg=assembly_information_model
    (your_env_name) python -m compas_rhino.install -p assembly_information_model

**XR Assembly**
::
    (your_env_name) python -m pip install git+https://github.com/augmentedfabricationlab/xr_assembly@master#egg=xr_assembly
    (your_env_name) python -m compas_rhino.install -p xr_assembly -v 7.0


Contributing
------------

Make sure you setup your local development environment correctly:

* Clone the `xr_assembly <https://github.com/augmentedfabricationlab/xr_assembly>`_ repository.
* Install development dependencies and make the project accessible from Rhino:

::

    pip install -r requirements-dev.txt
    invoke add-to-rhino

**You're ready to start working!**

During development, use tasks on the
command line to ease recurring operations:

* ``invoke clean``: Clean all generated artifacts.
* ``invoke check``: Run various code and documentation style checks.
* ``invoke docs``: Generate documentation.
* ``invoke test``: Run all tests and checks in one swift command.
* ``invoke add-to-rhino``: Make the project accessible from Rhino.
* ``invoke``: Show available tasks.

For more details, check the `Contributor's Guide <CONTRIBUTING.rst>`_.


Credits
-------------

This package was created by Lidia Atanasova <lidia.atanasova@tum.de> `@lidiatanasova <https://github.com/lidiatanasova>`_ at `@augmentedfabricationlab <https://github.com/augmentedfabricationlab>`_
