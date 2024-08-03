import { DataProvider } from "@refinedev/core";
import { axiosInstance, generateSort, generateFilter } from "./utils";
import { AxiosInstance } from "axios";
import queryString from "query-string";
import { stringify } from "qs";

type MethodTypes = "get" | "delete" | "head" | "options";
type MethodTypesWithBody = "post" | "put" | "patch";

export const BASE_ODATA_URL = "http://localhost:5000/odata";

const buildODataQuery = ({ pagination, filters, sorters, expand }: any) => {
  const query = {} as any;

  // Handle pagination
  if (pagination) {
    const { current = 1, pageSize = 10 } = pagination;
    query["$top"] = pageSize;
    query["$skip"] = (current - 1) * pageSize;
    query["$count"] = true;
  }

  // Handle filters
  if (filters && filters.length > 0) {
    const filterStrings = filters.map((filter: any) => {
      switch (filter.operator) {
        case "contains":
          return `contains(${filter.field},'${filter.value}')`;
        case "eq":
          return `${filter.field} eq '${filter.value}'`;
        // Add more cases for different operators if needed
        default:
          return `${filter.field} eq '${filter.value}'`;
      }
    });
    query["$filter"] = filterStrings.join(" and ");
  }

  // Handle sorters
  if (sorters && sorters.length > 0) {
    const sorterStrings = sorters.map(
      (sorter: any) => `${sorter.field} ${sorter.order}`
    );
    query["$orderby"] = sorterStrings.join(",");
  }

  if (expand) {
    query["$expand"] = expand;
  }

  return stringify(query, { addQueryPrefix: true });
};

export const odataProvider = (
  apiUrl: string,
  httpClient: AxiosInstance = axiosInstance
): Omit<
  Required<DataProvider>,
  "createMany" | "updateMany" | "deleteMany"
> => ({
  getList: async ({ resource, pagination, filters, sorters, meta }) => {
    const url = `${apiUrl}/${resource}`;

    const { current = 1, pageSize = 10 } = pagination ?? {};
    const query = buildODataQuery({
      filters,
      sorters,
      pagination: {
        current,
        pageSize,
      },
      select: meta?.select,
      expand: meta?.expand,
    });

    const { headers: headersFromMeta, method } = meta ?? {};
    const requestMethod = (method as MethodTypes) ?? "get";

    const { data } = await httpClient[requestMethod](`${url}${query}`, {
      headers: headersFromMeta,
    });
    return {
      data: data["value"],
      total: data["@odata.count"] || data.length,
    };
  },

  getMany: async ({ resource, ids, meta }) => {
    const { headers, method } = meta ?? {};
    const requestMethod = (method as MethodTypes) ?? "get";

    const { data } = await httpClient[requestMethod](
      `${apiUrl}/${resource}?${queryString.stringify({ id: ids })}`,
      { headers }
    );

    return {
      data,
    };
  },

  create: async ({ resource, variables, meta }) => {
    const url = meta?.childResource
      ? `${apiUrl}/${resource}/${meta.childResource}`
      : `${apiUrl}/${resource}`;
    const { headers, method } = meta ?? {};

    const requestMethod = (method as MethodTypesWithBody) ?? "post";

    const { data } = await httpClient[requestMethod](url, variables, {
      headers,
    });

    return {
      data,
    };
  },

  update: async ({ resource, id, variables, meta }) => {
    const url = meta?.childResource
      ? `${apiUrl}/${resource}/${id}/${meta.childResource}`
      : `${apiUrl}/${resource}/${id}`;

    const { headers, method } = meta ?? {};
    const requestMethod = (method as MethodTypesWithBody) ?? "patch";

    const { data } = await httpClient[requestMethod](url, variables, {
      headers,
    });

    return {
      data,
    };
  },

  getOne: async ({ resource, id, meta }) => {
    const url = `${apiUrl}/${resource}/${id}`;

    const { headers, method } = meta ?? {};
    const requestMethod = (method as MethodTypes) ?? "get";

    const { data } = await httpClient[requestMethod](url, { headers });

    return {
      data,
    };
  },

  deleteOne: async ({ resource, id, variables, meta }) => {
    const url = `${apiUrl}/${resource}/${id}`;

    const { headers, method } = meta ?? {};
    const requestMethod = (method as MethodTypesWithBody) ?? "delete";

    const { data } = await httpClient[requestMethod](url, {
      data: variables,
      headers,
    });

    return {
      data,
    };
  },

  getApiUrl: () => {
    return apiUrl;
  },

  custom: async ({
    url,
    method,
    filters,
    sorters,
    payload,
    query,
    headers,
  }) => {
    let requestUrl = `${url}?`;

    if (sorters) {
      const generatedSort = generateSort(sorters);
      if (generatedSort) {
        const { _sort, _order } = generatedSort;
        const sortQuery = {
          _sort: _sort.join(","),
          _order: _order.join(","),
        };
        requestUrl = `${requestUrl}&${queryString.stringify(sortQuery)}`;
      }
    }

    if (filters) {
      const filterQuery = generateFilter(filters);
      requestUrl = `${requestUrl}&${queryString.stringify(filterQuery)}`;
    }

    if (query) {
      requestUrl = `${requestUrl}&${queryString.stringify(query)}`;
    }

    let axiosResponse;
    switch (method) {
      case "put":
      case "post":
      case "patch":
        axiosResponse = await httpClient[method](url, payload, {
          headers,
        });
        break;
      case "delete":
        axiosResponse = await httpClient.delete(url, {
          data: payload,
          headers: headers,
        });
        break;
      default:
        axiosResponse = await httpClient.get(requestUrl, {
          headers,
        });
        break;
    }

    const { data } = axiosResponse;

    return Promise.resolve({ data });
  },
});
