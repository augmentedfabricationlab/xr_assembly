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
            'is_built': False,
            'is_support': False,
            'idx_v': None, # layer, used to compute keys_buildable
            'custom_attr_1': None, # placeholder for custom node attribute
            'custom_attr_2': None, # placeholder for custom node attribute
            'custom_attr_3': None # placeholder for custom node attribute
        })

    def get_neighbors_below(self, key):
        """
        get the direct neighbors below one chosen element
        """
        neighbors_below = self.network.neighbors_out(key)
        return neighbors_below

    def get_all_neighbors_below(self, key, all_keys_below=[]):
        """
        recursive function to get all neighbors below one chosen element
        """
        neighbors_below = self.get_neighbors_below(key)
        if len(neighbors_below):
            for n in neighbors_below:
                all_keys_below.append(n)
                self.get_all_neighbors_below(n, all_keys_below)
        return all_keys_below

    def get_keys_buildable(self):
        """
        return the keys of the elements which are buildable,
        i.e., the elements which are supported by the ground or by at least one elements below
        """

        keys_buildable = []

        for key in self.network.nodes_where({'is_built':False}):

            # check if the element is supported by the ground
            if self.network.node_attribute(key, 'idx_v') == 0:
                keys_buildable.append(key)

            # check if the element is supported by its both neighbors below
            else:
                neighbors_below = self.get_neighbors_below(key)
                supported = False
                if len(neighbors_below) > 0:
                    supported = all(self.network.node_attribute(n, 'is_built') == True for n in neighbors_below)
                if supported:
                    keys_buildable.append(key)
                else:
                    continue

        return keys_buildable
