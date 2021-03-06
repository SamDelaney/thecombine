import * as React from "react";
import { User } from "./user";
import { TempData, TempStep } from "../goals/tempGoal";

export enum GoalOption {
  Complete,
  Abandon,
  Current
}

export enum Tools {
  TempTool, // To be removed once testing is finished
  FixDups,
  CharInv,
  CharCreate
}

export type GoalData = TempData; // | OtherTypes
export type Steps = TempStep; // | OtherTypes

export interface Goals {
  id: number;
  name: String;
  user: User;

  steps: React.Component[];
  data: GoalData; // The data required to load/reload this exact goal
  display: React.FC;
  displaySelectorItem: React.FC;

  tool: Tools;
  completed: boolean;
  result: GoalOption;
}
