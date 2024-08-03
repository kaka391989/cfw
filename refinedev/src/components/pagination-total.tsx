import type { FC } from "react";
import { useTranslation } from "react-i18next";

type PaginationTotalProps = {
  total: number;
  entityName: string;
};

export const PaginationTotal: FC<PaginationTotalProps> = ({
  total,
  entityName,
}) => {
  const { t } = useTranslation();
  return (
    <span
      style={{
        marginLeft: "16px",
      }}
    >
      <span className="ant-text secondary">{t("table.totals", { total })}</span>
    </span>
  );
};
