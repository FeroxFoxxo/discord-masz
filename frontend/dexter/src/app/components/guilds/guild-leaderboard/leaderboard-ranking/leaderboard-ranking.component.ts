import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { CalcGuildUserLevel } from 'src/app/models/CalcGuildUserLevel';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-leaderboard-ranking',
  templateUrl: './leaderboard-ranking.component.html',
  styleUrls: ['./leaderboard-ranking.component.css']
})
export class LeaderboardRankingComponent implements OnInit {

  @Input("order") order : string = "total";

  TESTING = true;

  guildId = "0";
  page = 1;
  loading = false;
  loadedUsers: CalcGuildUserLevel[] = [];
  canLoadMoreUsersAfter = true;
  DEFAULT_PAGE_SIZE = 100;

  constructor(private api: ApiService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get("guildid") ?? "0";

    this.page = Number(this.route.snapshot.queryParamMap.get("page")) ?? 1;
    if (this.guildId == "0") return;

    this.loadMoreUsersAfter();
  }

  requestPage(page: number) : Observable<CalcGuildUserLevel[]> {
    this.loading = true;
    return this.api.getSimpleData(`/levels/guilds/${this.guildId}/users?order=${this.order}&page=${page}&pageSize=${this.DEFAULT_PAGE_SIZE}`);
  }

  loadMoreUsersBefore() {
    if (this.page <= 1) return;

    this.requestPage(this.page - 1).subscribe(data => {
      this.page--;
      for (let u of this.loadedUsers) {
        data.push(u);
      }
      this.loadedUsers = data;
      this.loading = false;
    })
  }

  loadMoreUsersAfter() {
    if (!this.canLoadMoreUsersAfter) return;

    let newPage = this.page - 1 + (this.loadedUsers.length / this.DEFAULT_PAGE_SIZE);
    this.requestPage(newPage).subscribe(data => {
      console.log(`loaded ${data.length} new users: ${JSON.stringify(data)}`)
      if (data.length < this.DEFAULT_PAGE_SIZE) this.canLoadMoreUsersAfter = false;

      for (let u of data) {
        this.loadedUsers.push(u);
      }
      this.loading = false;
    })
  }

  getPfpUrl(userId: string): string {
    return "";
  }

}
