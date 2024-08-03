import { useSelect } from "@refinedev/core";

export const useEmployeePositionSelect = () => {
  return useSelect({
    resource: "employees",
    dataProviderName: "odata",
    meta: {},
  });
};
