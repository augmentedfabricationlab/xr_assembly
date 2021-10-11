from __future__ import absolute_import
from __future__ import division
from __future__ import print_function


from assembly_information_model.assembly import Assembly
import math


class XRAssembly(Assembly):
    """A data structure for discrete element assemblies for vizualization in XR.
    """

    def __init__(self,
                 elements=None,
                 attributes=None,
                 default_element_attributes=None,
                 default_connection_attributes=None):

        super(XRAssembly, self).__init__()


        # add to default element attributes
        self.network.default_node_attributes.update({
            'is_built': False, #
            'is_support': False, #
        })
